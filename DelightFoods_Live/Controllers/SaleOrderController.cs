﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using System.Security.Claims;
using DelightFoods_Live.Models.DTO;

namespace DelightFoods_Live.Controllers
{
    public class SaleOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SaleOrder
        public async Task<IActionResult> Index()
        {
            return View(await _context.SaleOrder.ToListAsync());
        }

        // GET: SaleOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleOrderModel = await _context.SaleOrder
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleOrderModel == null)
            {
                return NotFound();
            }

            return View(saleOrderModel);
        }

        // GET: SaleOrder/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleOrderModel saleOrderModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saleOrderModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(saleOrderModel);
        }

        // GET: SaleOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleOrderModel = await _context.SaleOrder.FindAsync(id);
            if (saleOrderModel == null)
            {
                return NotFound();
            }
            return View(saleOrderModel);
        }

        // POST: SaleOrder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleOrderModel saleOrderModel)
        {
            if (id != saleOrderModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saleOrderModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleOrderModelExists(saleOrderModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(saleOrderModel);
        }

        // GET: SaleOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleOrderModel = await _context.SaleOrder
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleOrderModel == null)
            {
                return NotFound();
            }

            return View(saleOrderModel);
        }

        // POST: SaleOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saleOrderModel = await _context.SaleOrder.FindAsync(id);
            if (saleOrderModel != null)
            {
                _context.SaleOrder.Remove(saleOrderModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleOrderModelExists(int id)
        {
            return _context.SaleOrder.Any(e => e.Id == id);
        }




        [HttpPost]
        public JsonResult CreateOrderByCart(IEnumerable<CartDTO> model)
        {

            if (model != null && model.Any())
            {
                foreach (var item in model)
                {
                    var saleOrder = new SaleOrderModel();

                    saleOrder.ProductId = item.ProductId;
                    saleOrder.TotalPrice = item.TotalPrice;
                    saleOrder.Quantity = item.Quantity;
                    saleOrder.Status = 0;
                    saleOrder.ShippingId = 0;
                    saleOrder.CustomerId = item.CustomerId;
                    saleOrder.CreatedOnUTC = DateTime.UtcNow;
                    _context.SaleOrder.Add(saleOrder);
                }
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }


    }
}
