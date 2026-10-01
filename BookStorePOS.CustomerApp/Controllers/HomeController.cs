using Microsoft.AspNetCore.Mvc;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            var request = new BookListRequestModel { Page = page, Limit = 12, Title = search };
            var response = await _bookService.GetBooksAsync(request);
            
            return View(response);
        }
    }
}
