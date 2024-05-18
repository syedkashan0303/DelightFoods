using DelightFoods.APIs.Data;
using DelightFoods.APIs.Model;
using DelightFoods.APIs.Model.DTO;
using DelightFoods.APIs.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DelightFoods.APIs.Controllers
{
   [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var productModel = await _context.Products.ToListAsync();

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
            return Ok(ModelList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
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

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(ProductDTO productModel)
        {
            if (string.IsNullOrWhiteSpace(productModel.Name) || string.IsNullOrWhiteSpace(productModel.Description))
            {
                return BadRequest("Name and Description are required.");
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

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileNameWithPath = Path.Combine(path, file.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var mediaCenter = new MediaGalleryModel
                    {
                        Name = file.FileName,
                        CategoryId = 0,
                        ProductId = model.Id,
                        FilePath = fileNameWithPath,
                        CreatedOnUTC = DateTime.UtcNow
                    };
                    _context.MediaGallery.Add(mediaCenter);
                    await _context.SaveChangesAsync();
                }
            }
            return CreatedAtAction(nameof(GetProduct), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productModel)
        {
            if (id != productModel.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (string.IsNullOrWhiteSpace(productModel.Name) || string.IsNullOrWhiteSpace(productModel.Description))
            {
                return BadRequest("Name and Description are required.");
            }

            var utilities = new MapperClass<ProductDTO, ProductModel>();
            var model = utilities.Map(productModel);
            model.CreatedOnUTC = DateTime.UtcNow;
            _context.Update(model);

            try
            {
                await _context.SaveChangesAsync();

                if (productModel.UploadedFiles.Count > 0)
                {
                    var media = _context.MediaGallery.Where(x => x.ProductId == id);
                    if (media != null && media.Any())
                    {
                        foreach (var item in media)
                        {
                            _context.MediaGallery.Remove(item);
                        }
                        await _context.SaveChangesAsync();
                    }
                    foreach (var file in productModel.UploadedFiles)
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        string fileNameWithPath = Path.Combine(path, file.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var mediaCenter = new MediaGalleryModel
                        {
                            Name = file.FileName,
                            CategoryId = 0,
                            ProductId = model.Id,
                            FilePath = fileNameWithPath,
                            CreatedOnUTC = DateTime.UtcNow
                        };
                        _context.MediaGallery.Add(mediaCenter);
                        await _context.SaveChangesAsync();
                    }
                }
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        [HttpGet("shop")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Shop()
        {
            var productModel = await _context.Products.Where(x => x.Stock > 0).ToListAsync();

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
            return Ok(ModelList);
        }
    }
}
