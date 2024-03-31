using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DelightFoods_Live.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(
            ApplicationDbContext context ,
            UserManager<IdentityUser> userManager, 
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var CartProducts = _context.Cart;
            var product = CartProducts != null ? _context.Product.Where(x=> CartProducts.Select(z=>z.ProductId).Contains(x.Id)) : null;
            var cartList = new List<CartDTO>();
            foreach (var item in CartProducts)
            {
                var pro = product.Where(x => x.Id == item.ProductId).FirstOrDefault();
                cartList.Add(new CartDTO { 
                    Id = item.Id ,
                    ProductId = item.ProductId ,
                    ProductName = pro != null ? pro.Name : "",
                    ProductPrice = pro != null ? pro.Price : 0,
                    Quantity = item.Quantity
                });
            }
            return View(cartList);
        }

        public ActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddItemCart(CartModel model)
        {
            if (model.ProductId > 0)
            {
                var cart = _context.Cart.Where(x => x.ProductId == model.ProductId && x.CustomerId == model.CustomerId).FirstOrDefault();

                if (cart != null )
                {
                    cart.Quantity = cart.Quantity + 1;
                    cart.CreatedOnUTC = DateTime.UtcNow;
                    _context.Update(cart);
                }
                else
                {
                    ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
                    string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();

                    model.CustomerId = customer != null ? customer.Id : 0;
                    model.CreatedOnUTC = DateTime.UtcNow;
                    _context.Add(model);
                }
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }

        [HttpPost]
        public JsonResult RemoveItemFromCart(int id)
        {
            if (id > 0)
            {
                var cart = _context.Cart.FirstOrDefault(m => m.Id == id);

                if (cart == null)
                {
                    return Json("error");
                }

                cart.Quantity = cart.Quantity > 0  ? cart.Quantity - 1 : 0;

                _context.Update(cart);
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }

        [HttpPost]
        public JsonResult RemoveAllItemFromCart(int id)
        {
            if (id > 0)
            {
                var cart = _context.Cart.FirstOrDefault(m => m.Id == id);

                if (cart == null)
                {
                    return Json("error");
                }
                _context.Remove(cart);
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }


        [HttpGet]
        public JsonResult CartItem (int id)
        {
            if (id > 0)
            {
                var cart = _context.Cart.FirstOrDefault(m => m.Id == id);

                if (cart == null)
                {
                    return Json("error");
                }

                cart.Quantity = cart.Quantity > 0 ? cart.Quantity - 1 : 0;

                _context.Update(cart);
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }


    }
}
