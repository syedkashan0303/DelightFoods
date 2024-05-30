using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Utilites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DelightFoods_Live.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
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
                var category = categories != null && categories.Any() ? categories.FirstOrDefault(x => x.Id == item.CategoryId): null;
                var media = mediaFiles.Where(x => x.ProductId == item.Id).FirstOrDefault();

                model.CategoryName = category != null ? category.Name : "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";

                ModelList.Add(model);
            }
            return View(ModelList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Product.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<ProductModel, ProductDTO>();
            var model = utilities.Map(productModel);
            var allCategories = _context.Category.FirstOrDefault(x => x.Id == productModel.CategoryId);
            model.CategoryName = allCategories != null ? allCategories.Name : "";

            var mediaFile = _context.MediaGallery.Where(x => x.ProductId == model.Id);
            if (mediaFile != null && mediaFile.Any())
            {
                foreach (var item in mediaFile)
                {
                    var newfileList = new ProductDTO.MediaFiles();
                    var path = item.FilePath;
                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ProductDTO();

            var allCategories = _context.Category.ToList();
            model.categoryList = allCategories.Where(x=>x.ParentCategoryId > 0).Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO productModel)
        {
			if (string.IsNullOrWhiteSpace(productModel.Name) || string.IsNullOrWhiteSpace(productModel.Description))
			{
				var Model = new ProductDTO();

				var allCategories = _context.Category.ToList();
				Model.categoryList = allCategories.Where(x => x.ParentCategoryId > 0).Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToList();
				return View(Model);
			}

            var cat = _context.Product.Where(x => x.Name == productModel.Name);
            if (cat != null && cat.Any())
            {
                ModelState.AddModelError(string.Empty, "Name cannot be duplicate.");
                return View(productModel);
            }

            var utilities = new MapperClass<ProductDTO, ProductModel>();
            var model = utilities.Map(productModel);
            model.CreatedOnUTC = DateTime.UtcNow;
            _context.Add(model);
            await _context.SaveChangesAsync();

            if (productModel.UploadedFiles.Count > 0)
            {
                foreach (var file in productModel.UploadedFiles)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileNameWithPath = Path.Combine(path, file.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var mediaCenter = new MediaGalleryModel();

                    mediaCenter.Name = file.FileName;
                    mediaCenter.CategoryId = 0;
                    mediaCenter.ProductId = model.Id;
                    mediaCenter.FilePath = fileNameWithPath;
                    mediaCenter.CreatedOnUTC = DateTime.UtcNow;
                    _context.MediaGallery.Add(mediaCenter);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Product.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }
            var utilities = new MapperClass<ProductModel, ProductDTO>();
            var model = utilities.Map(productModel);
            var allCategories = _context.Category.ToList();
     
            
            model.categoryList = allCategories.Where(x=>x.ParentCategoryId > 0).Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == model.CategoryId
            }).ToList();

            model.ParentcategoryList = allCategories.Where(x => x.ParentCategoryId == 0).Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == model.CategoryId
            }).ToList();

            var mediaFile = _context.MediaGallery.Where(x => x.ProductId == model.Id);
            if (mediaFile != null && mediaFile.Any())
            {

                foreach (var item in mediaFile)
                {
                    var newfileList = new ProductDTO.MediaFiles();
                    var path = item.FilePath;
                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                    model.MediaFilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO productModel)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }
          
            try
            {
				if (string.IsNullOrWhiteSpace(productModel.Name) || string.IsNullOrWhiteSpace(productModel.Description))
				{
					var Model = new ProductDTO();

					var allCategories = _context.Category.ToList();
					Model.categoryList = allCategories.Where(x => x.ParentCategoryId > 0).Select(x => new SelectListItem()
					{
						Value = x.Id.ToString(),
						Text = x.Name
					}).ToList();

					var mediaFile = _context.MediaGallery.Where(x => x.ProductId == id);
					if (mediaFile != null && mediaFile.Any())
					{

						foreach (var item in mediaFile)
						{
							var newfileList = new ProductDTO.MediaFiles();
							var path = item.FilePath;
							newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
							newfileList.FileName = item.Name;
							newfileList.FileId = item.Id;
							Model.MediaFileList.Add(newfileList);
							Model.MediaFilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";
						}
					}

					return View(Model);
				}


                var cat = _context.Product.Where(x => x.Name == productModel.Name && x.Id != id);
                if (cat != null && cat.Any())
                {
                    ModelState.AddModelError(string.Empty, "Name cannot be duplicate.");
                    return View(productModel);
                }

                var utilities = new MapperClass<ProductDTO, ProductModel>();
                var model = utilities.Map(productModel);
                model.CreatedOnUTC = DateTime.Now;
                _context.Update(model);
                _context.SaveChanges();

				if (productModel.UploadedFiles.Count > 0)
				{
                    var media = _context.MediaGallery.Where(x => x.ProductId == id);
                    if (media != null && media.Any())
                    {
                        foreach (var item in media)
                        {
                            _context.MediaGallery.Remove(item);
                        }
						_context.SaveChanges();
					}
					foreach (var file in productModel.UploadedFiles)
					{
						string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

						//create folder if not exist
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);

						string fileNameWithPath = Path.Combine(path, file.FileName);

						using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
						{
							file.CopyTo(stream);
						}

						var mediaCenter = new MediaGalleryModel();

						mediaCenter.Name = file.FileName;
						mediaCenter.CategoryId = 0;
						mediaCenter.ProductId = model.Id;
						mediaCenter.FilePath = fileNameWithPath;
						mediaCenter.CreatedOnUTC = DateTime.UtcNow;
						_context.MediaGallery.Add(mediaCenter);
						_context.SaveChanges();
					}
				}

				return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelExists(productModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

       [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Product.FindAsync(id);
            if (productModel != null)
            {
                _context.Product.Remove(productModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Shop()
        {
            var productModel = await _context.Product.Where(x=>x.Stock > 0).ToListAsync();

            if (productModel == null)
            {
                return NotFound();
            }

            var ModelList = new List<ProductDTO>();
            var utilities = new MapperClass<ProductModel, ProductDTO>();

            var mediaFiles = _context.MediaGallery.Where(x => productModel.Select(z => z.Id).ToList().Contains(x.ProductId));
            var categories = _context.Category.ToList();


			foreach (var item in productModel)
            {
                var model = utilities.Map(item);
                var category = categories != null && categories.Any() ? categories.FirstOrDefault(x => x.Id == item.CategoryId) : null;
                var media = mediaFiles.Where(x => x.ProductId == item.Id).FirstOrDefault();

                model.CategoryName = category != null ? category.Name : "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";

                model.categoryList = categories.Where(x => x.ParentCategoryId > 0).Select(x => new SelectListItem()
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToList();

				ModelList.Add(model);
            }
            return View(ModelList);
        }
    }
}
