using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DelightFoods_Live.Data;
using DelightFoods_Live.Models;

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
            return View(await _context.SaleOrderModel.ToListAsync());
        }

        // GET: SaleOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleOrderModel = await _context.SaleOrderModel
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

        // POST: SaleOrder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductIds,CustomerId,Quantity,TotalPrice,Status,ShippingId,CreatedOnUTC")] SaleOrderModel saleOrderModel)
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

            var saleOrderModel = await _context.SaleOrderModel.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductIds,CustomerId,Quantity,TotalPrice,Status,ShippingId,CreatedOnUTC")] SaleOrderModel saleOrderModel)
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

            var saleOrderModel = await _context.SaleOrderModel
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
            var saleOrderModel = await _context.SaleOrderModel.FindAsync(id);
            if (saleOrderModel != null)
            {
                _context.SaleOrderModel.Remove(saleOrderModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleOrderModelExists(int id)
        {
            return _context.SaleOrderModel.Any(e => e.Id == id);
        }
    }
}
