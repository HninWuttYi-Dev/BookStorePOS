using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;
using Microsoft.AspNetCore.Http;

namespace BookStorePOS.MvcApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;
        private readonly IFileStorageService _fileStorageService;

        public BookController(IBookService bookService, ILogger<BookController> logger, IFileStorageService fileStorageService)
        {
            _bookService = bookService;
            _logger = logger;
            _fileStorageService = fileStorageService;
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
        public async Task<IActionResult> BookSaveAsync(BookCreateRequestModel requestModel, IFormFile? photo)
        {
            _logger.LogInformation("Book Save Async => Creating new book");

            if (photo != null && photo.Length > 0)
            {
                var url = await _fileStorageService.UploadImageAsync(photo);
                if (url != null)
                {
                    requestModel.CoverImageUrl = url;
                }
            }

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
        [ActionName("Edit")]
        public async Task<IActionResult> BookEditAsync(int id)
        {
            _logger.LogInformation($"Book Edit Async => Fetching book {id}");
            var model = await _bookService.GetBookAsync(new BookByIdRequestModel { BookId = id });
            
            if (!model.isSuccess)
            {
                _logger.LogWarning($"Book Edit Async => Failed to fetch book {id}: {model.Message}");
                TempData["isSuccess"] = false;
                TempData["Message"] = model.Message;
                return Redirect("/Book");
            }

            ViewData["Id"] = model.Data.BookId;
            ViewData["Title"] = model.Data.Title;
            ViewData["Author"] = model.Data.Author;
            ViewData["Genre"] = model.Data.Genre;
            ViewData["Isbn"] = model.Data.Isbn;
            ViewData["Price"] = model.Data.Price;
            ViewData["StockQuantity"] = model.Data.StockQuantity;
            ViewData["ReorderLevel"] = model.Data.ReorderLevel;
            ViewData["Description"] = model.Data.Description;

            return View("BookEdit", model.Data);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> BookUpdateAsync(int id, BookPatchRequestModel requestModel, IFormFile? photo)
        {
            _logger.LogInformation($"Book Update Async => Updating book {id}");
            requestModel.BookId = id;

            if (photo != null && photo.Length > 0)
            {
                var url = await _fileStorageService.UploadImageAsync(photo);
                if (url != null)
                {
                    requestModel.CoverImageUrl = url;
                }
            }

            if (string.IsNullOrWhiteSpace(requestModel.Title) 
                && string.IsNullOrWhiteSpace(requestModel.Author) 
                && string.IsNullOrWhiteSpace(requestModel.Genre)
                && requestModel.Price is null
                && requestModel.StockQuantity is null)
            {
                return Json(new BookPatchResponseModel { isSuccess = false, Message = "Please update at least one field." });
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                {
                    _logger.LogWarning("Book Update Async => ISBN must be 10 or 13 digits");
                    return Json(new BookPatchResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." });
                }
            }

            var model = await _bookService.UpdateBookAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Book Update Async => Book updated successfully");
                TempData["Message"] = model.Message ?? "Book updated successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Book Update Async => Failed to update book: {model.Message}");
            }

            return Json(model);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> BookDeleteAsync(BookDeleteRequestModel requestModel)
        {
            _logger.LogInformation($"Book Delete Async => Deleting book {requestModel.BookId}");
            var model = await _bookService.DeleteBookAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Book Delete Async => Book deleted successfully");
                TempData["Message"] = model.Message ?? "Book deleted successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Book Delete Async => Failed to delete book: {model.Message}");
            }
            return Json(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
