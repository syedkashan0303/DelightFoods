using System;
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
using DelightFoods_Live.Utilites;

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
            var saleOrderModel = await _context.SaleOrder.FirstOrDefaultAsync(m => m.Id == id);

            if (saleOrderModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
            var model = utilities.Map(saleOrderModel);
            var saleOrderProductMapping = _context.SaleOrderProductMapping.Where(z=>z.SaleOrderId == saleOrderModel.Id).ToList();
            var products = saleOrderProductMapping!= null && saleOrderProductMapping.Any() ? _context.Product.Where(x => saleOrderProductMapping.Select(z => z.ProductID).Contains(x.Id)).ToList(): null;

            foreach (var item in saleOrderProductMapping)
            {
                var product = products != null && products.Any() ? products.Where(x => x.Id == item.ProductID).FirstOrDefault(): null;

                model.saleOrderProductMappings.Add(new SaleOrderProductMappingDTO
                {

                    ProductID = item.ProductID,
                    ProductName = product != null ? product.Name : "" ,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Id = item.Id,
                    SaleOrderId = item.SaleOrderId
                });
            }


            return View(model);
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

                var saleOrder = new SaleOrderModel();

                saleOrder.PaymentId = 0;
                saleOrder.TotalPrice = model.Sum(x=>x.TotalPrice);
                saleOrder.Status = "Pending";
                saleOrder.ShippingId = 0;
                saleOrder.CustomerId = model.FirstOrDefault().CustomerId;
                saleOrder.CreatedOnUTC = DateTime.UtcNow;
                _context.SaleOrder.Add(saleOrder);
                _context.SaveChangesAsync();

                foreach (var item in model)
                {
                    var mapping = new SaleOrderProductMappingModel();
                    mapping.SaleOrderId = saleOrder.Id;
                    mapping.ProductID = item.ProductId;
                    mapping.Quantity = item.Quantity;
                    mapping.Price = item.ProductPrice;
                    _context.SaleOrderProductMapping.Add(mapping);
                    _context.SaveChangesAsync();
                }

                var carts = _context.Cart.Where(x => model.Select(z => z.Id).Contains(x.Id)).ToList();
                foreach (var item in carts)
                {
                    item.IsOrderCreated = true;
                    _context.Update(item);
                    _context.SaveChanges();
                }

                return Json("success");
            }
            return Json("error");
        }


        [HttpPost]
        public JsonResult CartProductPayment(CartDTO model)
        {
            if (model != null )
            {
                var payment = new PaymentModel();
                payment.CardNumber = "123456789";
                payment.Expiry = "23/22";
                payment.CVC = 123;
                _context.Payment.Add(payment);
                _context.SaveChanges();

                var carts = _context.Cart.Where(x => model.CartDTOlist.Select(z => z.Id).Contains(x.Id)).ToList();

                var saleOrder = carts != null && carts.Any() ? _context.SaleOrder.Where(x => x.CustomerId == (carts.FirstOrDefault().CustomerId)) : null ;
                foreach (var item in saleOrder)
                {
                    item.PaymentId = payment.Id;
                    _context.SaleOrder.Update(item);
                }
                foreach (var item in carts)
                {
                    _context.Cart.Remove(item);
                }
                    _context.SaveChanges();


                return Json("success");
            }
            return Json("error");
        }



    }
}
