using Microsoft.AspNetCore.Mvc;

namespace BookStore.MvcApp.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
