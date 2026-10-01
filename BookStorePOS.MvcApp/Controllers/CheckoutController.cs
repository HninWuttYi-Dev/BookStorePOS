using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;

namespace BookStorePOS.MvcApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IBookService bookService, ILogger<CheckoutController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> CheckoutAsync(int page = 1, string search = "")
        {
            _logger.LogInformation("Checkout Index => Fetching books");
            var request = new BookListRequestModel { Page = page, Limit = 12, Title = search };
            var response = await _bookService.GetBooksAsync(request);
            
            if (response.isSuccess && response.Data != null)
            {
                ViewData["Books"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["Search"] = search;
            }
            
            return View("Checkout");
        }
    }
}
