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
using DelightFoods_Live.Utilites;
using System.IO;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


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
            var saleOrderProductMapping = _context.SaleOrderProductMapping.Where(z => z.SaleOrderId == saleOrderModel.Id).ToList();
            var products = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? _context.Product.Where(x => saleOrderProductMapping.Select(z => z.ProductID).Contains(x.Id)).ToList() : null;

            foreach (var item in saleOrderProductMapping)
            {
                var product = products != null && products.Any() ? products.Where(x => x.Id == item.ProductID).FirstOrDefault() : null;

                model.saleOrderProductMappings.Add(new SaleOrderProductMappingDTO
                {

                    ProductID = item.ProductID,
                    ProductName = product != null ? product.Name : "",
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

                saleOrder.TotalPrice = model.Sum(x => x.TotalPrice);
                saleOrder.Status = OrderStatusEnum.Pending.ToString();
                saleOrder.ShippingId = 0;
                saleOrder.CustomerId = model.FirstOrDefault().CustomerId;
                saleOrder.CreatedOnUTC = DateTime.UtcNow;
                _context.Add(saleOrder);
                _context.SaveChanges();

                foreach (var item in model)
                {
                    var mapping = new SaleOrderProductMappingModel();
                    mapping.SaleOrderId = saleOrder.Id;
                    mapping.ProductID = item.ProductId;
                    mapping.Quantity = item.Quantity;
                    mapping.Price = item.ProductPrice;
                    _context.SaleOrderProductMapping.Add(mapping);
                    _context.SaveChanges();
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
            if (model != null)
            {

                try
                {
                    var carts = _context.Cart.Where(x => model.CartDTOlist.Select(z => z.Id).Contains(x.Id)).ToList();
                    var saleOrder = carts != null && carts.Any() ? _context.SaleOrder.Where(x => x.Id == carts.FirstOrDefault().OrderId)?.FirstOrDefault() ?? null : null;

                    saleOrder.Status = OrderStatusEnum.ReadytoShip.ToString();
                    _context.SaleOrder.Update(saleOrder);

                    var payment = new PaymentTransaction();
                    payment.IsCOD = model.IsCOD;
                    payment.OrderId = saleOrder.Id;
                    payment.CreatedOnUTC = DateTime.Now;
                    _context.PaymentTransaction.Add(payment);
                    _context.SaveChanges();
                    if (!payment.IsCOD)
                    {

                        var cardDetail = new CardDetailsModel();
                        cardDetail.CardholderName = "model.CardholderName";
                        cardDetail.PaymentId = payment.Id;
                        cardDetail.CardNumber = model.CardNumber;
                        cardDetail.Expiry = model.Expiry;
                        cardDetail.IsSave = false;
                        cardDetail.CVC = model.CVC;
                        cardDetail.CreatedOnUTC = DateTime.Now;
                        _context.CardDetails.Add(cardDetail);
                        _context.SaveChanges();

                    }


                    foreach (var item in carts)
                    {
                        _context.Cart.Remove(item);
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                    var abc = ex.Message;
                    throw;
                }

    

                return Json("success");
            }
            return Json("error");
        }

        public IActionResult GetSaleOrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleOrderModel = _context.SaleOrder.FirstOrDefault(m => m.Id == id);

            if (saleOrderModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
            var model = utilities.Map(saleOrderModel);
            var saleOrderProductMapping = _context.SaleOrderProductMapping.Where(z => z.SaleOrderId == saleOrderModel.Id).ToList();
            var products = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? _context.Product.Where(x => saleOrderProductMapping.Select(z => z.ProductID).Contains(x.Id)).ToList() : null;

            foreach (var item in saleOrderProductMapping)
            {
                var product = products != null && products.Any() ? products.FirstOrDefault(x => x.Id == item.ProductID) : null;

                model.saleOrderProductMappings.Add(new SaleOrderProductMappingDTO
                {
                    ProductID = item.ProductID,
                    ProductName = product != null ? product.Name : "",
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Id = item.Id,
                    SaleOrderId = item.SaleOrderId
                });
            }

            return Json(model);
        }

        public IActionResult GeneratePdf(int id)
        {
            var saleOrderModel = _context.SaleOrder.FirstOrDefault(m => m.Id == id);

            if (saleOrderModel == null)
            {
                return NotFound();
            }

            var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
            var model = utilities.Map(saleOrderModel);
            var _pdfGenerator = new PdfGenerator();

            var saleOrderProductMapping = _context.SaleOrderProductMapping.Where(z => z.SaleOrderId == saleOrderModel.Id).ToList();
            var products = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? _context.Product.Where(x => saleOrderProductMapping.Select(z => z.ProductID).Contains(x.Id)).ToList() : null;

            var customer = _context.Customers.Where(x => x.Id == model.CustomerId).FirstOrDefault();

            foreach (var item in saleOrderProductMapping)
            {
                var product = products != null && products.Any() ? products.Where(x => x.Id == item.ProductID).FirstOrDefault() : null;

                model.saleOrderProductMappings.Add(new SaleOrderProductMappingDTO
                {
                    ProductID = item.ProductID,
                    ProductName = product != null ? product.Name : "",
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Id = item.Id,
                    SaleOrderId = item.SaleOrderId
                });
            }
            model.CustomerName = customer != null ? customer.FirstName + " " + customer.LastName : "";
            var htmlContent = _pdfGenerator.GeneratePlainTextContent(model);

            var htmlFilePath = "SaleOrderPDF.html"; // Change the file extension to .html
            var pdfOutputPath = "PDF/pdf.pdf";

            // Save the generated HTML content to a file
            System.IO.File.WriteAllText(htmlFilePath, htmlContent);

            // Convert the HTML file to PDF
            _pdfGenerator.ConvertHtmlToPdf(htmlFilePath, pdfOutputPath);

            // Delete the temporary HTML file
            System.IO.File.Delete(htmlFilePath);

            // Return a file response with the generated PDF
            return File(System.IO.File.ReadAllBytes(pdfOutputPath), "application/pdf", pdfOutputPath);
        }

    }
}
