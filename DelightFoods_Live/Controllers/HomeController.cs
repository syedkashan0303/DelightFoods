using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Utilites;
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

		public async Task<IActionResult> Index()
		{
            return View();
        }

		public async Task<IActionResult> Dashboard()
		{
            var productModel = await _context.Product.ToListAsync();

            if (productModel == null)
            {
                return NotFound();
            }

            var ModelList = new List<ProductDTO>();
            var utilities = new MapperClass<ProductModel, ProductDTO>();

            var mediaFiles = _context.MediaGallery.Where(x => productModel.Select(z => z.Id).ToList().Contains(x.ProductId));
            var categories = _context.Category.Where(x => productModel.Select(z => z.CategoryId).Contains(x.Id)).ToList();

            foreach (var item in productModel)
            {
                var model = utilities.Map(item);
                var category = categories != null && categories.Any() ? categories.FirstOrDefault(x => x.Id == item.CategoryId) : null;
                var media = mediaFiles.Where(x => x.ProductId == item.Id).FirstOrDefault();

                model.CategoryName = category != null ? category.Name : "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";

                ModelList.Add(model);
            }
            return View(ModelList);
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
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }

    }
}
