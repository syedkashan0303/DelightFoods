using Microsoft.AspNetCore.Mvc;

namespace DelightFoods_Live.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
