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

        [ActionName("Create")]
        public IActionResult BookCreate()
        {
            return View("BookCreate");
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> BookSaveAsync(BookCreateRequestModel requestModel)
        {
            _logger.LogInformation("Book Save Async => Creating new book");

            if (string.IsNullOrWhiteSpace(requestModel.Title))
            {
                _logger.LogWarning("Book Save Async => Title is required");
                return Json(new BookCreateResponseModel { isSuccess = false, Message = "Title is required" });
            }
            if (string.IsNullOrWhiteSpace(requestModel.Author))
            {
                _logger.LogWarning("Book Save Async => Author is required");
                return Json(new BookCreateResponseModel { isSuccess = false, Message = "Author is required" });
            }
            if (string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                _logger.LogWarning("Book Save Async => Genre is required");
                return Json(new BookCreateResponseModel { isSuccess = false, Message = "Genre is required" });
            }
            if (requestModel.Price <= 0)
            {
                _logger.LogWarning("Book Save Async => Price must be greater than zero");
                return Json(new BookCreateResponseModel { isSuccess = false, Message = "Price must be greater than zero" });
            }
            if (requestModel.StockQuantity < 0)
            {
                _logger.LogWarning("Book Save Async => StockQuantity must be greater or zero");
                return Json(new BookCreateResponseModel { isSuccess = false, Message = "StockQuantity must be greater or equal to zero" });
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                {
                    _logger.LogWarning("Book Save Async => ISBN must be 10 or 13 digits");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." });
                }
            }

            BookCreateResponseModel model = await _bookService.CreateBookAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Book Save Async => Book created successfully");
                TempData["Message"] = model.Message ?? "Book created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Book Save Async => Failed to create book: {model.Message}");
            }

            return Json(model);
        }
    }
}
