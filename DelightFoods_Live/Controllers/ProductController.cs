﻿using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Utilites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DelightFoods_Live.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
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
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            //ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id");

            var model = new ProductDTO();

            var allCategories = _context.Category.ToList();
            model.categoryList = allCategories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return View(model);
        }
        [Authorize(Roles = "Admin")]

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO productModel)
        {
            if (ModelState.IsValid)
            {
                var utilities = new MapperClass<ProductDTO, ProductModel>();
                var model = utilities.Map(productModel);
                model.CreatedOnUTC = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productModel);
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

                var utilities = new MapperClass<ProductDTO, ProductModel>();
                var model = utilities.Map(productModel);

                _context.Update(model);
                await _context.SaveChangesAsync();

                //var allCategories = _context.Category.ToList();

                //productModel.categoryList = allCategories.Select(x => new SelectListItem()
                //{
                //    Value = x.Id.ToString(),
                //    Text = x.Name,
                //    Selected = x.Id == productModel.CategoryId
                //}).ToList();

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
    }
}
