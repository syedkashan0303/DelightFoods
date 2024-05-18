using DelightFoods.APIs.Data;
using DelightFoods.APIs.Model;
using DelightFoods.APIs.Model.DTO;
using DelightFoods.APIs.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DelightFoods.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
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
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";
                ModelList.Add(model);
            }
            return Ok(ModelList);
        }

        // GET: api/Category/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var categoryModel = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

        //    if (categoryModel == null)
        //    {
        //        return NotFound();
        //    }

        //    var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
        //    var model = utilities.Map(categoryModel);

        //    var childCategories = _context.Category.Where(x => x.ParentCategoryId == categoryModel.Id).ToList();

        //    if (childCategories != null && childCategories.Any())
        //    {
        //        model.ChildCategoryList = childCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        //    }

        //    var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
        //    if (mediaFile != null && mediaFile.Any())
        //    {
        //        foreach (var item in mediaFile)
        //        {
        //            var newfileList = new CategoryModelDTO.MediaFiles();
        //            var path = item.FilePath;
        //            newfileList.FilePath = path.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/");
        //            newfileList.FileName = item.Name;
        //            newfileList.FileId = item.Id;
        //            model.MediaFileList.Add(newfileList);
        //        }
        //    }
        //    return Ok(model);
        //}

        // POST: api/Category
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CategoryModelDTO categoryModel)
        //{
        //    if (string.IsNullOrWhiteSpace(categoryModel.Name) || string.IsNullOrWhiteSpace(categoryModel.Description))
        //    {
        //        return BadRequest("Name and Description are required");
        //    }
        //    var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
        //    var category = utilities.Map(categoryModel);

        //    category.CreatedByUTC = DateTime.UtcNow;
        //    _context.Category.Add(category);
        //    _context.SaveChanges();

        //    if (categoryModel.UploadedFiles.Count > 0)
        //    {
        //        foreach (var file in categoryModel.UploadedFiles)
        //        {
        //            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);

        //            string fileNameWithPath = Path.Combine(path, file.FileName);

        //            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }

        //            var mediaCenter = new MediaGalleryModel
        //            {
        //                Name = file.FileName,
        //                CategoryId = category.Id,
        //                ProductId = 0,
        //                FilePath = fileNameWithPath,
        //                CreatedOnUTC = DateTime.UtcNow
        //            };
        //            _context.MediaGallery.Add(mediaCenter);
        //            _context.SaveChanges();
        //        }
        //    }
        //    return CreatedAtAction(nameof(Details), new { id = category.Id }, categoryModel);
        //}

        // PUT: api/Category/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit(int id, [FromBody] CategoryModelDTO categoryModel)
        //{
        //    if (string.IsNullOrWhiteSpace(categoryModel.Name) || string.IsNullOrWhiteSpace(categoryModel.Description))
        //    {
        //        return BadRequest("Name and Description are required");
        //    }

        //    var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
        //    var category = utilities.Map(categoryModel);
        //    if (category != null)
        //    {
        //        _context.Update(category);
        //        await _context.SaveChangesAsync();
        //    }
        //    return NoContent();
        //}

        //// DELETE: api/Category/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var categoryModel = await _context.Category.FindAsync(id);
        //    if (categoryModel != null)
        //    {
        //        _context.Category.Remove(categoryModel);
        //        await _context.SaveChangesAsync();
        //    }
        //    return NoContent();
        //}

        [HttpGet("ChildCategoryList")]
        public async Task<IActionResult> ChildCategoryList()
        {
            var categoryModel = await _context.Category.ToListAsync();

            if (categoryModel == null)
            {
                return NotFound();
            }

            var ModelList = new List<CategoryModelDTO>();
            var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();

            var mediaFiles = _context.MediaGallery.Where(x => categoryModel.Select(z => z.Id).ToList().Contains(x.CategoryId));

            foreach (var item in categoryModel.Where(c => c.ParentCategoryId > 0))
            {
                var model = utilities.Map(item);
                var media = mediaFiles.Where(x => x.CategoryId == item.Id).FirstOrDefault();
                model.ParentCategoryName = categoryModel.FirstOrDefault(x => x.Id == item.ParentCategoryId)?.Name ?? "";
                model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";
                ModelList.Add(model);
            }
            return Ok(ModelList);
        }

        //[HttpGet("ChildCategoryDetails/{id}")]
        //public async Task<IActionResult> ChildCategoryDetails(int id)
        //{
        //    var categoryModel = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

        //    if (categoryModel == null)
        //    {
        //        return NotFound();
        //    }

        //    var utilities = new MapperClass<CategoryModel, CategoryModelDTO>();
        //    var model = utilities.Map(categoryModel);

        //    var ParentCategory = await _context.Category.FirstOrDefaultAsync(x => x.Id == categoryModel.ParentCategoryId);
        //    model.ParentCategoryName = ParentCategory != null ? ParentCategory.Name : "";

        //    var mediaFile = _context.MediaGallery.Where(x => x.CategoryId == categoryModel.Id);
        //    if (mediaFile != null && mediaFile.Any())
        //    {
        //        foreach (var item in mediaFile)
        //        {
        //            var newfileList = new CategoryModelDTO.MediaFiles
        //            {
        //                FilePath = item.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/"),
        //                FileName = item.Name,
        //                FileId = item.Id
        //            };
        //            model.MediaFileList.Add(newfileList);
        //        }
        //    }
        //    return Ok(model);
        //}

        //[HttpPut("ChildCategoryEdit/{id}")]
        //public async Task<IActionResult> ChildCategoryEdit(int id, [FromBody] CategoryModelDTO categoryModel)
        //{
        //    if (string.IsNullOrWhiteSpace(categoryModel.Name) || string.IsNullOrWhiteSpace(categoryModel.Description))
        //    {
        //        var model = new CategoryModelDTO();
        //        var parentCategory = _context.Category.Where(x => x.ParentCategoryId == 0);
        //        model.ParentCategoryList = parentCategory.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        //        return BadRequest("Name and Description are required");
        //    }

        //    var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
        //    var category = utilities.Map(categoryModel);
        //    if (category != null)
        //    {
        //        _context.Update(category);
        //        await _context.SaveChangesAsync();
        //    }
        //    return NoContent();
        //}

        //[HttpPost("CreateSubCategory")]
        //public async Task<IActionResult> CreateSubCategory([FromBody] CategoryModelDTO categoryModel)
        //{
        //    if (string.IsNullOrWhiteSpace(categoryModel.Name) || string.IsNullOrWhiteSpace(categoryModel.Description))
        //    {
        //        var model = new CategoryModelDTO();
        //        var parentCategory = _context.Category.Where(x => x.ParentCategoryId == 0);
        //        model.ParentCategoryList = parentCategory.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        //        return BadRequest("Name and Description are required");
        //    }

        //    var utilities = new MapperClass<CategoryModelDTO, CategoryModel>();
        //    var category = utilities.Map(categoryModel);

        //    category.CreatedByUTC = DateTime.UtcNow;
        //    _context.Category.Add(category);
        //    _context.SaveChanges();

        //    if (categoryModel.UploadedFiles.Count > 0)
        //    {
        //        foreach (var file in categoryModel.UploadedFiles)
        //        {
        //            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);

        //            string fileNameWithPath = Path.Combine(path, file.FileName);

        //            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }

        //            var mediaCenter = new MediaGalleryModel
        //            {
        //                Name = file.FileName,
        //                CategoryId = category.Id,
        //                ProductId = 0,
        //                FilePath = fileNameWithPath,
        //                CreatedOnUTC = DateTime.UtcNow
        //            };
        //            _context.MediaGallery.Add(mediaCenter);
        //            _context.SaveChanges();
        //        }
        //    }
        //    return CreatedAtAction(nameof(ChildCategoryDetails), new { id = category.Id }, categoryModel);
        //}
    }
}
