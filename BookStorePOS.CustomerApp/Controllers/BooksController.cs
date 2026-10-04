using Microsoft.AspNetCore.Mvc;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IHttpClientFactory httpClientFactory, ILogger<BooksController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        
        [ActionName("Index")]
        public async Task<IActionResult> BookListAsync(BookListRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation("Book List Async => Fetching books");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&SearchQuery={Uri.EscapeDataString(requestModel.SearchQuery ?? "")}&Isbn={Uri.EscapeDataString(requestModel.Isbn ?? "")}&Title={Uri.EscapeDataString(requestModel.Title ?? "")}&Author={Uri.EscapeDataString(requestModel.Author ?? "")}&Genre={Uri.EscapeDataString(requestModel.Genre ?? "")}";
                var httpResponse = await client.GetAsync($"api/book{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);

                if (response != null && response.isSuccess && response.Data != null)
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
                    _logger.LogWarning($"Book List Async => Failed to fetch books: {response?.Message}");
                    TempData["Message"] = response?.Message ?? "Failed to fetch books.";
                    TempData["isSuccess"] = false;
                }

                return View("BookList");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookListAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("Detail")]
        public async Task<IActionResult> BookDetailAsync(int id)
        {
        try
        {
                _logger.LogInformation($"Book Detail Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (response == null || !response.isSuccess || response.Data == null)
                {
                    _logger.LogWarning($"Book Detail Async => Failed to fetch book {id}");
                    TempData["Message"] = "Book not found.";
                    TempData["isSuccess"] = false;
                    return RedirectToAction("Index");
                }

                ViewData["Book"] = response.Data;
                return View("BookDetail");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookDetailAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
    }
}
