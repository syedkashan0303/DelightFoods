using DelightFoods_Live.Data;
using DelightFoods_Live.Models;
using DelightFoods_Live.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DelightFoods_Live.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
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
                var cart = _context.Cart.FirstOrDefault(x => x.ProductId == model.ProductId && x.CustomerId == model.CustomerId);

                if (cart != null )
                {
                    cart.Quantity = cart.Quantity + 1;
                    _context.Update(cart);
                }
                else
                {
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

    }
}
