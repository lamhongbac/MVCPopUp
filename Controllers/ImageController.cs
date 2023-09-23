using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MVCPopUp.Models;

namespace MVCPopUp.Controllers
{
    public class ImageController : Controller
    {
        private readonly TransactionDbContext _context;
        IWebHostEnvironment _hostEnvironment;
        public ImageController(TransactionDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            

        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.ToListAsync());
        }
        public async Task<IActionResult> UploadPopUp()
        {
            ImageModel model = new ImageModel();
            return View(model);
        }
        /// <summary>
        /// upload popUp required return json
        /// </summary>
        /// <param name="imageModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPopUp([Bind("Title,ImageFile")] ImageModel imageModel)
        {
            //can phai clear de en-force kiem tra lai model state
            ModelState.Clear();
            //Save image to wwwroot/image
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
            string extension = Path.GetExtension(imageModel.ImageFile.FileName);
            imageModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            if (ModelState.IsValid)
            {
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageModel.ImageFile.CopyToAsync(fileStream);
                }
                //Insert record
                _context.Add(imageModel);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Images.ToList()) });
            }
            else

                return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "UploadPopUp", imageModel) });
        }
        public async Task<IActionResult> Upload()
        {
            ImageModel model=new ImageModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([Bind("Title,ImageFile")] ImageModel imageModel)
        {
            ModelState.Clear();
            //Save image to wwwroot/image
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
            string extension = Path.GetExtension(imageModel.ImageFile.FileName);
            imageModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            if (ModelState.IsValid)
            {
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageModel.ImageFile.CopyToAsync(fileStream);
                }
                //Insert record
                _context.Add(imageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(imageModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var imageModel = await _context.Images.FindAsync(id);
           
            return View(imageModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ImageModel imageModel1)
        {
            var imageModel = await _context.Images.FindAsync(imageModel1.ImageId);
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", imageModel.ImageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            _context.Images.Remove(imageModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}
