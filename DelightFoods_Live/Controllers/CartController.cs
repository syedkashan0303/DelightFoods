using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using DelightFoods_Live.Utilites;
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
            ApplicationDbContext context,
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
            var product = CartProducts != null ? _context.Product.Where(x => CartProducts.Select(z => z.ProductId).Contains(x.Id)) : null;
            var cartList = new List<CartDTO>();
            foreach (var item in CartProducts)
            {
                var pro = product.Where(x => x.Id == item.ProductId).FirstOrDefault();

                cartList.Add(new CartDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = pro != null ? pro.Name : "",
                    ProductPrice = pro != null ? pro.Price : 0,
                    Quantity = item.Quantity
                });
            }
            return View(cartList);
        }

        [HttpGet]
        public ActionResult Cart()
        {
            var cartList = new CartDTO();
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var utilities = new MapperClass<CartModel, CartDTO>();
            var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();
            var CustomerId = customer != null ? customer.Id : 0;
            if (CustomerId > 0)
            {
                var cart = _context.Cart.Where(m => m.CustomerId == CustomerId).ToList();
                var product = cart != null ? _context.Product.Where(x => cart.Select(z => z.ProductId).Contains(x.Id)).ToList() : null;
                var mediaFiles = product != null ? _context.MediaGallery.Where(x => product.Select(z => z.Id).ToList().Contains(x.ProductId)).ToList() : null;
                if (cart == null)
                {
                    return View(cartList);
                }
                var iscreatedAlready = false;
                foreach (var item in cart)
                {
                    var pro = product.Where(x => x.Id == item.ProductId).FirstOrDefault();

                    var media = pro != null  ? mediaFiles.Where(x => x.ProductId == pro.Id).FirstOrDefault() : null;

                    var model = utilities.Map(item);
                    model.ProductName = pro != null ? pro.Name : "";
                    model.ProductPrice = pro != null ? pro.Price : 0;
                    model.TotalPrice = model.ProductPrice * model.Quantity;
                    model.MediaFilePath = media?.FilePath.Split(new string[] { "wwwroot" }, StringSplitOptions.None)[1].Replace("\\", "/") ?? "/img/default.png";
                    iscreatedAlready = !iscreatedAlready ? item.IsOrderCreated :true;
                    cartList.CartDTOlist.Add(model);
                }

                cartList.IsOrderCreated = iscreatedAlready;
                return View(cartList);

            }
            return View(cartList);
        }

        [HttpPost]
        public JsonResult AddItemCart(CartModel model)
        {

            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();


            if (model.ProductId > 0 && customer != null )
            {
                var cart = _context.Cart.Where(x => x.ProductId == model.ProductId && x.CustomerId == customer.Id).FirstOrDefault();

                if (cart != null)
                {
                    cart.Quantity = cart.Quantity + 1;
                    cart.CreatedOnUTC = DateTime.UtcNow;
                    _context.Update(cart);
                }
                else
                {
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

                cart.Quantity = cart.Quantity > 0 ? cart.Quantity - 1 : 0;
                if (cart.Quantity == 0 )
                {
                    _context.Remove(cart);
                }
                else
                {
                    _context.Update(cart);
                }
                _context.SaveChangesAsync();

                return Json("succes");
            }
            return Json("error");
        }

        [HttpPost]
        public JsonResult AddItemFromCart(int id)
        {
            if (id > 0)
            {
                var cart = _context.Cart.FirstOrDefault(m => m.Id == id);

                if (cart == null)
                {
                    return Json("error");
                }

                cart.Quantity = cart.Quantity + 1; 
               
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
        public JsonResult CartItemCount()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customer = _context.Customers.Where(x => x.UserId == userId).FirstOrDefault();

            if (customer != null && customer.Id > 0)
            {
                var cart = _context.Cart.Where(m => m.CustomerId == customer.Id);

                if (cart == null)
                {
                    return Json("0");
                }
                return Json(cart.Count().ToString());
            }
            return Json("0");
        }


    }
}
