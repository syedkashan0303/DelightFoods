using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DelightFoods_Live.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger , ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _context = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult DownloadFile(string fileName, int FileId, int CatetoryId = 0 , int productId = 0)
        {
            // Get the path to the file in wwwroot
            var mediaFile = new MediaGalleryModel();

            if (CatetoryId > 0)
            {
                mediaFile = _context.MediaGallery.FirstOrDefault(x => x.CategoryId == CatetoryId && x.Name == fileName && x.Id == FileId);
            }

            if (productId > 0)
            { 
                mediaFile = _context.MediaGallery.FirstOrDefault(x => x.ProductId == productId && x.Name == fileName && x.Id == FileId);
            }

            var filePath =  !string.IsNullOrEmpty(mediaFile.FilePath) ? mediaFile.FilePath : "" ;


            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                return PhysicalFile(filePath, "application/octet-stream", Path.GetFileName(filePath));
            }

            return NotFound();

        }

    }
}
