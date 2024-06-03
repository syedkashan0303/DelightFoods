namespace DelightFoods_Live.Controllers
{
	using DelightFoods_Live.Data;
	using DelightFoods_Live.Models;
	using DelightFoods_Live.Models.DTO;
	using DelightFoods_Live.Utilites;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines the <see cref="SaleOrderController" />
	/// </summary>
	public class SaleOrderController : Controller
	{
		/// <summary>
		/// Defines the _context
		/// </summary>
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Defines the _httpContextAccessor
		/// </summary>
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Initializes a new instance of the <see cref="SaleOrderController"/> class.
		/// </summary>
		/// <param name="context">The context<see cref="ApplicationDbContext"/></param>
		/// <param name="httpContextAccessor">The httpContextAccessor<see cref="IHttpContextAccessor"/></param>
		public SaleOrderController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		// GET: SaleOrder

		/// <summary>
		/// The Index
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> Index()
		{
			ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
			string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();

			var returnOrderList = _context.ReturnOrder.Where(x => x.CustomerID == customer.Id).ToList();

			var order = returnOrderList != null && returnOrderList.Any() ?
				 await _context.SaleOrder.Where(x => x.CustomerId == customer.Id && !returnOrderList.Select(x => x.OrderId).Contains(x.Id) && (x.Status != "ReadytoShip" || x.Status != "Returned")).ToListAsync()
				:
				 await _context.SaleOrder.Where(x => x.CustomerId == customer.Id && (x.Status != "ReadytoShip" || x.Status != "Returned")).ToListAsync()
				;
			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
			foreach (var item in order)
			{
				var validDate = item.CreatedOnUTC.AddDays(1);
				var shipping = _context.Shipping.Where(x => x.Id == item.ShippingId).FirstOrDefault();
				var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).OrderByDescending(x => x.Id);
				var model = utilities.Map(item);
				model.ShippingAddress = shipping != null ? shipping.Address : "Not Available";
				model.RemainingPayment = item.TotalPrice - payment.Sum(x => x.Amount);
				model.AdvancePayment = payment.Sum(x => x.Amount);
				model.PaymentType = payment != null && payment.Any() ? payment.FirstOrDefault().IsCOD ? "Cash on delivery" : "Card payment" : "Pending";
				model.IsReturnDateIsValde = validDate >= DateTime.Now;
				orderlist.Add(model);
			}

			return View(orderlist);
		}


		public async Task<IActionResult> OrderListForAdmin()
		{
			var order = _context.SaleOrder.OrderBy(x=>x.Status).ToList();
			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
			foreach (var item in order)
			{
				var validDate = item.CreatedOnUTC.AddDays(1);
				var shipping = _context.Shipping.Where(x => x.Id == item.ShippingId).FirstOrDefault();
				var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).OrderByDescending(x => x.Id);
				var model = utilities.Map(item);
				model.ShippingAddress = shipping != null ? shipping.Address : "Not Available";
				model.RemainingPayment = item.TotalPrice - payment.Sum(x => x.Amount);
				model.AdvancePayment = payment.Sum(x => x.Amount);
				model.PaymentType = payment != null && payment.Any() ? payment.FirstOrDefault().IsCOD ? "Cash on delivery" : "Card payment" : "Pending";
				model.IsReturnDateIsValde = validDate >= DateTime.Now;
				orderlist.Add(model);
			}
			return View(orderlist);
		}

		// GET: SaleOrder

		/// <summary>
		/// The ReadyForShipOrders
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> ReadyForShipOrders()
		{
			ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
			string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();

			var order = await _context.SaleOrder.Where(x => x.CustomerId == customer.Id && x.Status == "ReadytoShip").ToListAsync();

			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
			foreach (var item in order)
			{
				var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).Sum(x => x.Amount);
				var model = utilities.Map(item);
				model.AdvancePayment = payment;
				orderlist.Add(model);
			}

			return View(orderlist);
		}

		// GET: SaleOrder/Details/5

		/// <summary>
		/// The ReadyForShipOrderDetail
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> ReadyForShipOrderDetail(int? id)
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

			var payment = _context.PaymentTransaction.Where(x => x.OrderId == model.Id).FirstOrDefault();

			foreach (var item in saleOrderProductMapping)
			{
				var product = products != null && products.Any() ? products.Where(x => x.Id == item.ProductID).FirstOrDefault() : null;
				model.AdvancePayment = payment != null ? payment.Amount : 0;
				model.RemainingPayment = payment != null ? model.TotalPrice - payment.Amount : 0;

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

		// GET: SaleOrder/Details/5

		/// <summary>
		/// The Details
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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
			model.TotalPrice = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? saleOrderProductMapping.Sum(x => x.Price) : 0;
			model.TaxRate = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? Convert.ToString(saleOrderProductMapping.Sum(x => x.Price) * 0.18M) : "0";
			model.WithHoldingTax = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? Convert.ToString(Convert.ToDecimal(model.TaxRate) + saleOrderProductMapping.Sum(x => x.Price)) : "0";

			var payment = _context.PaymentTransaction.Where(x => x.OrderId == saleOrderModel.Id).OrderByDescending(x => x.Id);

			model.PaymentType = payment != null && payment.Any() ? payment.FirstOrDefault().IsCOD ? "Cash on delivery" : "Card payment" : "Pending";
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

		/// <summary>
		/// The Create
		/// </summary>
		/// <returns>The <see cref="IActionResult"/></returns>
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// The Create
		/// </summary>
		/// <param name="saleOrderModel">The saleOrderModel<see cref="SaleOrderModel"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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

		/// <summary>
		/// The Edit
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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

		/// <summary>
		/// The Edit
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <param name="saleOrderModel">The saleOrderModel<see cref="SaleOrderModel"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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

		/// <summary>
		/// The Delete
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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

		/// <summary>
		/// The DeleteConfirmed
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
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

		/// <summary>
		/// The SaleOrderModelExists
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="bool"/></returns>
		private bool SaleOrderModelExists(int id)
		{
			return _context.SaleOrder.Any(e => e.Id == id);
		}

		/// <summary>
		/// The CreateOrderByCart
		/// </summary>
		/// <param name="model">The model<see cref="IEnumerable{CartDTO}"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult CreateOrderByCart(IEnumerable<CartDTO> model)
		{
			if (model != null && model.Any())
			{
				var saleOrder = new SaleOrderModel();

				var taxAmmount = model.Sum(x => x.TotalPrice) * 0.18;

				saleOrder.TotalPrice = Convert.ToInt32(model.Sum(x => x.TotalPrice) + taxAmmount);
				saleOrder.Status = OrderStatusEnum.Pending.ToString();
				saleOrder.ShippingId = 0;
				saleOrder.address = "";
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
					item.OrderId = saleOrder.Id;
					_context.Update(item);
					_context.SaveChanges();
				}

				return Json("success");
			}
			return Json("error");
		}

		/// <summary>
		/// The CartProductPayment
		/// </summary>
		/// <param name="model">The model<see cref="CartDTO"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult CartProductPayment(CartDTO model)
		{
			if (model != null)
			{
				try
				{
					var carts = _context.Cart.Where(x => model.CartDTOlist.Select(z => z.Id).Contains(x.Id)).ToList();
					var saleOrder = carts != null && carts.Any() ? _context.SaleOrder.Where(x => x.Id == carts.FirstOrDefault().OrderId)?.FirstOrDefault() ?? null : null;
					if (saleOrder == null)
					{
						return Json("error");
					}

					saleOrder.Status = saleOrder != null ? OrderStatusEnum.Processing.ToString() : OrderStatusEnum.Pending.ToString();
					saleOrder.address = model.CustomerAddress;
					_context.SaleOrder.Update(saleOrder);

					var payment = new PaymentTransaction();
					payment.IsCOD = model.IsCOD;
					payment.OrderId = saleOrder.Id;
					payment.Amount = model.TotalPriceWithTax * 0.30m;
					payment.CreatedOnUTC = DateTime.Now;
					_context.PaymentTransaction.Add(payment);
					_context.SaveChanges();

					var cardDetail = new CardDetailsModel();
					cardDetail.CardholderName = model.CardholderName;
					cardDetail.PaymentId = payment.Id;
					cardDetail.CardNumber = model.CardNumber;
					cardDetail.Expiry = model.Expiry;
					cardDetail.IsSave = false;
					cardDetail.CVC = model.CVC;
					cardDetail.CreatedOnUTC = DateTime.Now;
					_context.CardDetails.Add(cardDetail);
					_context.SaveChanges();

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

		/// <summary>
		/// The RemainingProductPayment
		/// </summary>
		/// <param name="model">The model<see cref="SaleOrderDTO"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult RemainingProductPayment(SaleOrderDTO model)
		{
			if (model != null)
			{
				try
				{
					var saleOrder = _context.SaleOrder.Where(x => x.Id == model.Id)?.FirstOrDefault() ?? null;
					if (saleOrder == null)
					{
						return Json("error");
					}

					saleOrder.Status = model.Cashondelivery ? OrderStatusEnum.WaitingForPaymentConfirmation.ToString() : OrderStatusEnum.Shipped.ToString();
					_context.SaleOrder.Update(saleOrder);

					var payment = new PaymentTransaction();
					payment.IsCOD = model.Cashondelivery;
					payment.OrderId = saleOrder.Id;
					payment.Amount = model.RemainingPayment;
					payment.CreatedOnUTC = DateTime.Now;
					_context.PaymentTransaction.Add(payment);
					_context.SaveChanges();
					if (!model.Cashondelivery)
					{
						var cardDetail = new CardDetailsModel();
						cardDetail.CardholderName = model.CardholderName;
						cardDetail.PaymentId = payment.Id;
						cardDetail.CardNumber = model.CardNumber;
						cardDetail.Expiry = model.Expiry;
						cardDetail.IsSave = false;
						cardDetail.CVC = model.CVC;
						cardDetail.CreatedOnUTC = DateTime.Now;
						_context.CardDetails.Add(cardDetail);
						_context.SaveChanges();
					}
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

		/// <summary>
		/// The AdminCODOrderList
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> AdminCODOrderList()
		{
			// Fetch the COD payments
			var codPayments = _context.PaymentTransaction.Where(x => x.IsCOD).ToList();

			// Check if codPayments is null or empty before proceeding
			if (codPayments == null || !codPayments.Any())
			{
				return View(new List<SaleOrderDTO>()); // Return an empty list if there are no COD payments
			}

			// Fetch orders corresponding to the COD payments
			var orderIds = codPayments.Select(z => z.OrderId).ToList();
			var orders = _context.SaleOrder.Where(x => orderIds.Contains(x.Id)).ToList();

			// Check if orders is null or empty before proceeding
			if (orders == null || !orders.Any())
			{
				return View(new List<SaleOrderDTO>()); // Return an empty list if there are no orders
			}

			// Fetch customers corresponding to the orders
			var customerIds = orders.Select(z => z.CustomerId).ToList();
			var customers = _context.Customers.Where(x => customerIds.Contains(x.Id)).ToList();

			// Initialize the list for the resulting order DTOs
			var orderList = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();

			// Iterate through the orders and map them to the DTOs
			foreach (var order in orders.Where(x => x.Status == "WaitingForPaymentConfirmation"))
			{
				var payment = _context.PaymentTransaction.Where(x => x.OrderId == order.Id).Sum(x => x.Amount);
				var customer = customers.FirstOrDefault(x => x.Id == order.CustomerId);

				var model = utilities.Map(order);
				model.AdvancePayment = payment;
				model.CustomerName = customer != null ? customer.FirstName + " " + customer.LastName : "";
				model.CreatedStringDate = order.CreatedOnUTC.ToString("dd-MMMM-yyyy");

				orderList.Add(model);
			}

			// Return the view with the order list
			return View(orderList);
		}

		/// <summary>
		/// The GetSaleOrderDetails
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="IActionResult"/></returns>
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

		/// <summary>
		/// The GeneratePdf
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="IActionResult"/></returns>
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
			model.TotalPrice = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? saleOrderProductMapping.Sum(x => x.Price) : 0;
			model.TaxRate = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? Convert.ToString(saleOrderProductMapping.Sum(x => x.Price) * 0.18M) : "0";
			model.WithHoldingTax = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? Convert.ToString(Convert.ToDecimal(model.TaxRate) + saleOrderProductMapping.Sum(x => x.Price)) : "0";
			var payment = _context.PaymentTransaction.Where(x => x.OrderId == saleOrderModel.Id).OrderByDescending(x => x.Id);
			model.PaymentType = payment != null && payment.Any() ? payment.FirstOrDefault().IsCOD ? "Cash on delivery" : "Card payment" : "Pending";

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

		/// <summary>
		/// The DeleteOrderFromCart
		/// </summary>
		/// <param name="model">The model<see cref="IEnumerable{CartDTO}"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult DeleteOrderFromCart(IEnumerable<CartDTO> model)
		{
			if (model != null && model.Any())
			{
				var carts = _context.Cart.Where(x => model.Select(z => z.Id).Contains(x.Id)).ToList();

				var saleOrder = carts != null && carts.Any() ? _context.SaleOrder.Where(x => x.Id == carts.FirstOrDefault().OrderId) : null;

				var saleOrderMapping = carts != null && carts.Any() ? _context.SaleOrderProductMapping.Where(x => x.SaleOrderId == carts.FirstOrDefault().OrderId) : null;

				foreach (var item in saleOrder)
				{
					_context.SaleOrder.Remove(item);
				}
				_context.SaveChanges();

				foreach (var item in saleOrderMapping)
				{
					_context.SaleOrderProductMapping.Remove(item);
				}
				_context.SaveChanges();

				foreach (var item in carts)
				{
					item.IsOrderCreated = false;
					item.OrderId = 0;
					_context.Cart.Update(item);
				}
				_context.SaveChanges();

				return Json("success");
			}
			return Json("error");
		}

		/// <summary>
		/// The AdminOrderList
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> AdminOrderList()
		{
			var order = await _context.SaleOrder.ToListAsync();
			var customers = _context.Customers.Where(x => order.Select(z => z.CustomerId).Contains(x.Id)).ToList();

			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();

			foreach (var item in order.Where(x => x.Status == "Processing" || x.Status == "ReadytoShip" || x.Status == "Shipped"))
			{
				var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).Sum(x => x.Amount);
				var customer = customers != null ? customers.Where(x => x.Id == item.CustomerId).FirstOrDefault() : null;
				var model = utilities.Map(item);
				model.AdvancePayment = payment;
				model.CustomerName = customer != null ? customer.FirstName + " " + customer.LastName : "";
				orderlist.Add(model);
			}

			return View(orderlist);
		}

		/// <summary>
		/// The PaymentConfirm
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult PaymentConfirm(int id)
		{
			var order = _context.SaleOrder.FirstOrDefault(c => c.Id == id);

			if (order != null)
			{
				var customer = _context.Customers.Where(z => z.Id == order.CustomerId).FirstOrDefault();

				order.Status = OrderStatusEnum.Shipped.ToString();
				_context.Update(order);
				_context.SaveChanges();

				return Json("success");
			}

			return Json("error");
		}

		/// <summary>
		/// The AdminReturnOrderList
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> AdminReturnOrderList()
		{
			var returnOrderList = _context.ReturnOrder.ToList();
			var order = _context.SaleOrder.Where(x => returnOrderList.Select(z => z.OrderId).Contains(x.Id)).OrderByDescending(x => x.Id).ToList();

			var customers = _context.Customers.Where(x => order.Select(z => z.CustomerId).Contains(x.Id)).ToList();

			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
			if (order != null && order.Any())
			{
				foreach (var item in order)
				{
					var returnOrder = returnOrderList.Where(x => x.OrderId == item.Id).FirstOrDefault();
					var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).Sum(x => x.Amount);
					var customer = customers != null ? customers.Where(x => x.Id == item.CustomerId).FirstOrDefault() : null;
					var model = utilities.Map(item);
					model.CustomerName = customer != null ? customer.FirstName + " " + customer.LastName : "";
					model.Reason = returnOrder?.Reason;
					model.ReturnDate = returnOrder.CreatedOnUTC.ToString("dd/MMMM/yyyy");
					orderlist.Add(model);
				}
			}

			return View(orderlist);
		}

		// GET: SaleOrder/Details/5

		/// <summary>
		/// The ReturnDetails
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> ReturnDetails(int? id)
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
			var returnOrder = _context.ReturnOrder.Where(x => x.OrderId == saleOrderModel.Id).FirstOrDefault();
			var customer = _context.Customers.Where(x => x.Id == saleOrderModel.CustomerId).FirstOrDefault();
			var customerAddress = _context.Shipping.Where(x => x.Id == saleOrderModel.ShippingId).FirstOrDefault();
			var saleOrderProductMapping = _context.SaleOrderProductMapping.Where(z => z.SaleOrderId == saleOrderModel.Id).ToList();
			var products = saleOrderProductMapping != null && saleOrderProductMapping.Any() ? _context.Product.Where(x => saleOrderProductMapping.Select(z => z.ProductID).Contains(x.Id)).ToList() : null;
			model.Reason = returnOrder != null ? returnOrder.Reason : "";
			model.CustomerName = customer != null ? customer.FirstName + " " + customer.LastName : "";
			model.ShippingAddress = customerAddress != null ? customerAddress.Address : "";
			model.CreatedStringDate = returnOrder.CreatedOnUTC.ToString("dd-MMMM-yyyy");
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

		/// <summary>
		/// The ReadyToShip
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult ReadyToShip(int id)
		{
			var order = _context.SaleOrder.FirstOrDefault(c => c.Id == id);

			if (order != null)
			{
				var customer = _context.Customers.Where(z => z.Id == order.CustomerId).FirstOrDefault();

				var address = customer != null ? _context.CustomerAddress.Where(z => z.Id == customer.AddressId).FirstOrDefault() : null;

				var shipping = new ShippingModel();
				shipping.CreatedOnUTC = DateTime.UtcNow;
				shipping.Address = order.address;
				_context.Add(shipping);
				_context.SaveChanges();

				order.Status = OrderStatusEnum.ReadytoShip.ToString();
				order.ShippingId = shipping.Id;
				_context.Update(order);
				_context.SaveChanges();

				return Json("success");
			}

			return Json("error");
		}

		/// <summary>
		/// The ReturnApprove
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult ReturnApprove(int id)
		{
			var order = _context.SaleOrder.FirstOrDefault(c => c.Id == id);

			if (order != null)
			{
				var returnOrder = _context.ReturnOrder.Where(x => x.OrderId == order.Id).FirstOrDefault();
				if (returnOrder != null)
				{
					returnOrder.IsApproved = true;

					_context.Update(returnOrder);
					_context.SaveChanges();

					order.Status = OrderStatusEnum.Returned.ToString();
					_context.Update(order);
					_context.SaveChanges();
					return Json("success");
				}
			}
			return Json("error");
		}

		/// <summary>
		/// The ConfirmDelivery
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <returns>The <see cref="JsonResult"/></returns>
		[HttpPost]
		public JsonResult ConfirmDelivery(int id)
		{
			var order = _context.SaleOrder.FirstOrDefault(c => c.Id == id);

			if (order != null)
			{
				order.Status = OrderStatusEnum.Delivered.ToString();
				_context.Update(order);
				_context.SaveChanges();

				return Json("success");
			}

			return Json("error");
		}

		/// <summary>
		/// The ReturnForm
		/// </summary>
		/// <param name="id">The id<see cref="int?"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> ReturnForm(int? id)
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
			var model = new ReturnModelDTO();
			model.TotalPrice = saleOrderModel.TotalPrice;
			model.Status = saleOrderModel.Status;
			model.OrderId = saleOrderModel.Id;
			model.CustomerID = saleOrderModel.CustomerId;
			return View(model);
		}

		/// <summary>
		/// The ReturnForm
		/// </summary>
		/// <param name="id">The id<see cref="int"/></param>
		/// <param name="returnModel">The returnModel<see cref="ReturnModelDTO"/></param>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ReturnForm(int id, ReturnModelDTO returnModel)
		{
			if (id != returnModel.Id)
			{
				return NotFound();
			}

			if (returnModel.CustomerID > 0 && returnModel.OrderId > 0)
			{
				var model = new ReturnOrderModel();
				model.OrderId = returnModel.OrderId;
				model.CustomerID = returnModel.CustomerID;
				model.IsApproved = false;
				model.Reason = returnModel.Reason;
				model.CreatedOnUTC = DateTime.Now;
				_context.ReturnOrder.Add(model);
				_context.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		/// <summary>
		/// The ReturnOrderList
		/// </summary>
		/// <returns>The <see cref="Task{IActionResult}"/></returns>
		public async Task<IActionResult> ReturnOrderList()
		{
			ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
			string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();

			var returnOrderList = _context.ReturnOrder.Where(x => x.CustomerID == customer.Id);

			var order = returnOrderList != null && returnOrderList.Any() ? _context.SaleOrder.Where(x => returnOrderList.Select(z => z.OrderId).Contains(x.Id)).ToList() : null;

			var orderlist = new List<SaleOrderDTO>();
			var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();
			if (order != null && order.Any())
			{
				foreach (var item in order.Where(x => x.Status == "Returned"))
				{
					var shipping = _context.Shipping.Where(x => x.Id == item.ShippingId).FirstOrDefault();
					var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).Sum(x => x.Amount);
					var returnOrder = returnOrderList.Where(x => x.OrderId == item.Id).FirstOrDefault();
					var model = utilities.Map(item);
					model.ShippingAddress = shipping != null ? shipping.Address : "Not Available";
					model.TotalPrice = item.TotalPrice;
					model.Status = item.Status;
					model.Reason = returnOrder.Reason;
					model.ReturnDate = returnOrder.CreatedOnUTC.ToString("dd/MMMM/yyyy");
					orderlist.Add(model);
				}
			}
			return View(orderlist);
		}
	}
}
