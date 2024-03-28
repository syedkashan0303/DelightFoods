using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using Microsoft.AspNetCore.Authorization;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Utilites;

namespace DelightFoods_Live.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[Authorize(Roles = "Admin")]
        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categoryModel = await _context.Category.ToListAsync();

            if (categoryModel == null)
            {
                return NotFound();
            }

            var ModelList = new List<CategoryModelDTO>();
            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();

            var mediaFiles = _context.MediaGallery.Where(x => categoryModel.Select(z => z.Id).ToList().Contains(x.CategoryId));

            foreach (var item in categoryModel.Where(c => c.ParentCategoryId == 0))
            {
                var model = utilities.Map(item);

                var media = mediaFiles.Where(x => x.CategoryId == item.Id).FirstOrDefault();

                model.ParentCategoryName = categoryModel.FirstOrDefault(x => x.Id == item.ParentCategoryId)?.Name ?? "";
                //model.MediaFilePath = media?.FilePath.Split("wwwroot/")[1].Replace('\' , '/')?? "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "";

                ModelList.Add(model);
            }
            return View(ModelList);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

            if (categoryModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
            var model = utilities.Map(categoryModel);

            var childCategories = _context.Category.Where(x => x.ParentCategoryId == categoryModel.Id).ToList();

            if (childCategories != null  && childCategories.Any())
            {
                model.ChildCategoryList = childCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            }
            var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
            if (mediaFile != null && mediaFile.Any())
            {
                foreach (var item in mediaFile)
                {
                    var newfileList = new CategoryModelDTO.MediaFiles();
                    var path = item.FilePath;
                    //newfileList.FileBytes = System.IO.File.ReadAllBytes(path);

                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                }
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CategoryModelDTO categoryModel)
        {
            var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
            var category = utilities.Map(categoryModel);

            category.CreatedByUTC = DateTime.UtcNow;
            _context.Category.Add(category);
            _context.SaveChanges();

            if (categoryModel.UploadedFiles.Count > 0)
            {
                foreach (var file in categoryModel.UploadedFiles)
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
                    mediaCenter.CategoryId = category.Id;
                    mediaCenter.ProductId = 0;
                    mediaCenter.FilePath = fileNameWithPath;
                    mediaCenter.CreatedOnUTC = DateTime.UtcNow;
                    _context.MediaGallery.Add(mediaCenter);
                    _context.SaveChanges();
                }
            }
            return View(categoryModel);
        }

        //[Authorize(Roles = "Admin")]
        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Category.FindAsync(id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
            var model = utilities.Map(categoryModel);

            var childCategories = _context.Category.Where(x => x.ParentCategoryId == categoryModel.Id).ToList();

            if (childCategories != null && childCategories.Any())
            {
                model.ChildCategoryList = childCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            }


            var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
            if (mediaFile != null && mediaFile.Any())
            {
                foreach (var item in mediaFile)
                {
                    var newfileList = new CategoryModelDTO.MediaFiles();
                    var path = item.FilePath;
                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                }
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModelDTO categoryModel)
        {
            var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
            var category = utilities.Map(categoryModel);
            if (category != null)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            return View(category);
        }

        //[Authorize(Roles = "Admin")]
        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryModel = await _context.Category.FindAsync(id);
            if (categoryModel != null)
            {
                _context.Category.Remove(categoryModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryModelExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        #region

        //[Authorize(Roles = "Admin")]
        // GET: Category/Create

        public async Task<IActionResult> ChildCategoryList()
        {
            var categoryModel = await _context.Category.ToListAsync();

            if (categoryModel == null)
            {
                return NotFound();
            }

            var ModelList = new List<CategoryModelDTO>();
            var utilities = new MapperClass<CategoryModel , CategoryModelDTO>();

            var mediaFiles = _context.MediaGallery.Where(x=> categoryModel.Select(z=>z.Id).ToList().Contains(x.CategoryId));

            foreach (var item in categoryModel.Where(c => c.ParentCategoryId > 0))
            {
                var model = utilities.Map(item);

                var media = mediaFiles.Where(x => x.CategoryId == item.Id).FirstOrDefault();

                model.ParentCategoryName = categoryModel.FirstOrDefault(x => x.Id == item.ParentCategoryId)?.Name ?? "";
                //model.MediaFilePath = media?.FilePath.Split("wwwroot/")[1].Replace('\' , '/')?? "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "";

                ModelList.Add(model);
            }
            return View(ModelList);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChildCategoryDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

            if (categoryModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
            var model = utilities.Map(categoryModel);

            var ParentCategory = await _context.Category.FirstOrDefaultAsync(x => x.Id == categoryModel.ParentCategoryId);

            
            model.ParentCategoryName = ParentCategory!= null ? ParentCategory.Name : "";
            var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
            if (mediaFile != null && mediaFile.Any())
            {
                foreach (var item in mediaFile)
                {
                    var newfileList = new CategoryModelDTO.MediaFiles();
                    var path = item.FilePath;
                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");

                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ChildCategoryEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Category.FindAsync(id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
            var model = utilities.Map(categoryModel);

            var parentCategory = _context.Category.Where(x => x.ParentCategoryId == 0);

            model.ParentCategoryList = parentCategory.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name ,Selected = x.Id == model.ParentCategoryId }).ToList();

            var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
            if (mediaFile != null && mediaFile.Any())
            {
                foreach (var item in mediaFile)
                {
                    var newfileList = new CategoryModelDTO.MediaFiles();
                    var path = item.FilePath;
                    newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    newfileList.FileName = item.Name;
                    newfileList.FileId = item.Id;
                    model.MediaFileList.Add(newfileList);
                }
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChildCategoryEdit(int id, CategoryModelDTO categoryModel)
        {
            var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
            var category = utilities.Map(categoryModel);
            if (category != null)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            return View(category);
        }

        public IActionResult CreateSubCategory()
        {
            var model = new CategoryModelDTO();

            var parentCategory = _context.Category.Where(x => x.ParentCategoryId == 0);

            model.ParentCategoryList = parentCategory.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubCategory(CategoryModelDTO categoryModel)
        {
            var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
            var category = utilities.Map(categoryModel);

            category.CreatedByUTC = DateTime.UtcNow;
            _context.Category.Add(category);
            _context.SaveChanges();


            if (categoryModel.UploadedFiles.Count > 0)
            {
                foreach (var file in categoryModel.UploadedFiles)
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
                    mediaCenter.CategoryId = category.Id;
                    mediaCenter.ProductId = 0;
                    mediaCenter.FilePath = fileNameWithPath;
                    mediaCenter.CreatedOnUTC = DateTime.UtcNow;
                    _context.MediaGallery.Add(mediaCenter);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("ChildCategoryList");
        }

        #endregion
    }
}
