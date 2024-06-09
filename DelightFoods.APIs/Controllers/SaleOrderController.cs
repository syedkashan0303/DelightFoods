using DelightFoods.APIs.Data;
using DelightFoods.APIs.Model;
using DelightFoods.APIs.Model.DTO;
using DelightFoods.APIs.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DelightFoods.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
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

        [HttpGet("OrderListForAdmin")]
        public async Task<IActionResult> OrderListForAdmin()
        {
            try
            {
                var orders = _context.SaleOrder.OrderBy(x => x.Status).ToList();
                var orderList = new List<SaleOrderDTO>();
                var utilities = new MapperClass<SaleOrderModel, SaleOrderDTO>();

                foreach (var item in orders)
                {
                    var validDate = item.CreatedOnUTC.AddDays(1);
                    var shipping = _context.Shipping.Where(x => x.Id == item.ShippingId).FirstOrDefault();
                    var payment = _context.PaymentTransaction.Where(x => x.OrderId == item.Id).OrderByDescending(x => x.Id).ToList();
                    var model = utilities.Map(item);

                    model.ShippingAddress = shipping != null ? shipping.Address : "Not Available";
                    model.RemainingPayment = item.TotalPrice - payment.Sum(x => x.Amount);
                    model.AdvancePayment = payment.Sum(x => x.Amount);
                    model.PaymentType = payment != null && payment.Any() ? payment.FirstOrDefault().IsCOD ? "Cash on delivery" : "Card payment" : "Pending";
                    model.IsReturnDateIsValde = validDate >= DateTime.Now;

                    orderList.Add(model);
                }

                return Ok(orderList);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
