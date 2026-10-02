using Microsoft.AspNetCore.Mvc;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }
        
        [ActionName("Index")]
        public async Task<IActionResult> BookListAsync(BookListRequestModel requestModel)
        {
            _logger.LogInformation("Book List Async => Fetching books");

            // Use Title from layout search if it was mistakenly passed as "search", though we will fix layout too.
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
                ViewData["SearchQuery"] = requestModel.SearchQuery;
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
