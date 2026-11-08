using System.Net;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Shared.Models.Common;

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
        public async Task<IActionResult> BookListAsync(BookListRequestModel requestModel, [FromQuery] string? filter = null)
        {
            try
            {
                _logger.LogInformation("Book List Async => Fetching books. Filter: {Filter}", filter);

                var client = _httpClientFactory.CreateClient("WebAPI");
                List<BookModel> books = new();
                int currentPage = requestModel.Page <= 0 ? 1 : requestModel.Page;
                int totalPages = 1;
                HashSet<int> bestsellerBookIds = new();

                // Always kick off a background fetch for top bestseller IDs so catalog cards can display the BEST SELLER badge
                var bestsellersTask = client.GetAsync("api/book/bestsellers?days=30&limit=12");

                if (!string.IsNullOrEmpty(filter) && filter.Equals("trending", StringComparison.OrdinalIgnoreCase))
                {
                    var trendingResp = await client.GetAsync("api/book/trending?days=7&limit=36");
                    var jsonString = await trendingResp.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Result<List<BookModel>>>(jsonString);

                    if (response != null && response.IsSuccess && response.Data != null)
                    {
                        var allTrending = response.Data;
                        if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
                        {
                            var q = requestModel.SearchQuery.Trim();
                            allTrending = allTrending.Where(b =>
                                (b.Title != null && b.Title.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                                (b.Author != null && b.Author.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                                (b.Editions != null && b.Editions.Any(e => e.Isbn != null && e.Isbn.Contains(q, StringComparison.OrdinalIgnoreCase)))
                            ).ToList();
                        }

                        int pageSize = 12;
                        totalPages = Math.Max(1, (int)Math.Ceiling(allTrending.Count / (double)pageSize));
                        currentPage = Math.Min(currentPage, totalPages);
                        books = allTrending.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    }

                    ViewData["Filter"] = "trending";
                    ViewData["FilterTitle"] = "New & Trending Books";
                }
                else if (!string.IsNullOrEmpty(filter) && filter.Equals("bestsellers", StringComparison.OrdinalIgnoreCase))
                {
                    var bsResp = await bestsellersTask;
                    var jsonString = await bsResp.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Result<List<BookModel>>>(jsonString);

                    if (response != null && response.IsSuccess && response.Data != null)
                    {
                        var allBestsellers = response.Data;
                        if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
                        {
                            var q = requestModel.SearchQuery.Trim();
                            allBestsellers = allBestsellers.Where(b =>
                                (b.Title != null && b.Title.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                                (b.Author != null && b.Author.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                                (b.Editions != null && b.Editions.Any(e => e.Isbn != null && e.Isbn.Contains(q, StringComparison.OrdinalIgnoreCase)))
                            ).ToList();
                        }

                        int pageSize = 12;
                        totalPages = Math.Max(1, (int)Math.Ceiling(allBestsellers.Count / (double)pageSize));
                        currentPage = Math.Min(currentPage, totalPages);
                        books = allBestsellers.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                        bestsellerBookIds = allBestsellers.Select(b => b.BookId).ToHashSet();
                    }

                    ViewData["Filter"] = "bestsellers";
                    ViewData["FilterTitle"] = "Recent Bestsellers";
                }
                else
                {
                    var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&SearchQuery={Uri.EscapeDataString(requestModel.SearchQuery ?? "")}&Isbn={Uri.EscapeDataString(requestModel.Isbn ?? "")}&Title={Uri.EscapeDataString(requestModel.Title ?? "")}&Author={Uri.EscapeDataString(requestModel.Author ?? "")}&Genre={Uri.EscapeDataString(requestModel.Genre ?? "")}";
                    var httpResponse = await client.GetAsync($"api/book{qs}");
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);

                    if (response != null && response.IsSuccess && response.Data != null)
                    {
                        books = response.Data;
                        currentPage = response.Page;
                        totalPages = response.TotalPages;
                    }
                    else
                    {
                        _logger.LogWarning($"Book List Async => Failed to fetch books: {response?.Message}");
                        TempData["Message"] = response?.Message ?? "Failed to fetch books.";
                        TempData["isSuccess"] = false;
                    }
                }

                // Populate bestsellerBookIds if not already populated
                if (bestsellerBookIds.Count == 0)
                {
                    try
                    {
                        var bsResp = await bestsellersTask;
                        if (bsResp.IsSuccessStatusCode)
                        {
                            var jsonString = await bsResp.Content.ReadAsStringAsync();
                            var bsResult = JsonConvert.DeserializeObject<Result<List<BookModel>>>(jsonString);
                            if (bsResult?.Data != null)
                            {
                                bestsellerBookIds = bsResult.Data.Select(b => b.BookId).ToHashSet();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to load bestseller IDs for badge overlay");
                    }
                }

                ViewData["Books"] = books;
                ViewData["CurrentPage"] = currentPage;
                ViewData["TotalPages"] = totalPages;
                ViewData["IsbnFilter"] = requestModel.Isbn;
                ViewData["TitleFilter"] = requestModel.Title;
                ViewData["AuthorFilter"] = requestModel.Author;
                ViewData["GenreFilter"] = requestModel.Genre;
                ViewData["SearchQuery"] = requestModel.SearchQuery;
                ViewData["Filter"] = filter;
                ViewData["BestsellerBookIds"] = bestsellerBookIds;
                _logger.LogInformation("Book List Async => Books fetched successfully. Count: {Count}", books.Count);

                return View("BookList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookListAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet]
        [ActionName("LoadMore")]
        public async Task<IActionResult> LoadMoreBooksAsync([FromQuery] BookCursorRequestModel requestModel)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Cursor={requestModel.Cursor}&Limit={requestModel.Limit}&SearchQuery={Uri.EscapeDataString(requestModel.SearchQuery ?? "")}&Author={Uri.EscapeDataString(requestModel.Author ?? "")}&Genre={Uri.EscapeDataString(requestModel.Genre ?? "")}";
                var httpResponse = await client.GetAsync($"api/book/cursor{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                return Content(jsonString, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadMoreBooksAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = ex.Message });
            }
        }

        [ActionName("Detail")]
        public async Task<IActionResult> BookDetailAsync(int id)
        {
            try
            {
                if (id <= 0 || !ModelState.IsValid)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Book Detail Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (response == null || !response.IsSuccess || response.Data == null)
                {
                    _logger.LogWarning($"Book Detail Async => Failed to fetch book {id}");
                    return NotFound();
                }

                ViewData["Book"] = response.Data;
                return View("BookDetail");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookDetailAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
