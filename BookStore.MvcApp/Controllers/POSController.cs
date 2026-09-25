using Microsoft.AspNetCore.Mvc;

namespace BookStore.MvcApp.Controllers
{
    public class POSController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
