using Microsoft.AspNetCore.Mvc;

namespace BookStore.MvcApp.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
