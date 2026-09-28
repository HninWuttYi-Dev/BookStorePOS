using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;

namespace BookStorePOS.MvcApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> BookListAsync(BookListRequestModel requestModel)
        {
            _logger.LogInformation("Book List Async => Fetching books");

            var response = await _bookService.GetBooksAsync(requestModel);

            if (response.isSuccess && response.Data != null)
            {
                ViewData["Books"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["IsbnFilter"] = requestModel.Isbn;
                ViewData["TitleFilter"] = requestModel.Title;
                ViewData["AuthorFilter"] = requestModel.Author;
                ViewData["GenreFilter"] = requestModel.Genre;
                _logger.LogInformation("Book List Async => Books fetched successfully");
            }
            else
            {
                _logger.LogWarning($"Book List Async => Failed to fetch books: {response.Message}");
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
            }

            return View("BookList");
        }
    }
}
