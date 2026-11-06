using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Author;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IHttpClientFactory httpClientFactory, ILogger<AuthorsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index(string search = "", int limit = 6)
        {
            try
            {
                _logger.LogInformation("Authors Controller => Fetching initial author shelves via cursor, search: {Search}", search);

                var client = _httpClientFactory.CreateClient("WebAPI");
                var encodedSearch = Uri.EscapeDataString(search ?? "");
                var httpResponse = await client.GetAsync($"api/author/cursor?AuthorName={encodedSearch}&Limit={limit}");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<AuthorCursorResponseModel>(jsonString);

                var authors = response?.Data ?? new List<AuthorModel>();

                ViewData["Authors"] = authors;
                ViewData["Search"] = search;
                ViewData["NextCursor"] = response?.NextCursor;
                ViewData["HasNextPage"] = response?.HasNextPage ?? false;
                ViewData["TotalCount"] = response?.TotalCount ?? authors.Count;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authors Controller => Exception occurred while fetching initial authors");
                ViewData["Authors"] = new List<AuthorModel>();
                ViewData["Search"] = search;
                ViewData["NextCursor"] = null;
                ViewData["HasNextPage"] = false;
                ViewData["TotalCount"] = 0;
                return View();
            }
        }

        [HttpGet]
        [ActionName("LoadMore")]
        public async Task<IActionResult> LoadMore(int? cursor, string search = "", int limit = 6)
        {
            try
            {
                _logger.LogInformation("Authors Controller => Loading more author shelves with cursor: {Cursor}", cursor);

                var client = _httpClientFactory.CreateClient("WebAPI");
                var encodedSearch = Uri.EscapeDataString(search ?? "");
                var cursorParam = cursor.HasValue ? $"&Cursor={cursor.Value}" : "";
                var httpResponse = await client.GetAsync($"api/author/cursor?AuthorName={encodedSearch}&Limit={limit}{cursorParam}");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<AuthorCursorResponseModel>(jsonString);

                return Json(new
                {
                    isSuccess = response?.IsSuccess ?? false,
                    data = response?.Data ?? new List<AuthorModel>(),
                    nextCursor = response?.NextCursor,
                    hasNextPage = response?.HasNextPage ?? false,
                    totalCount = response?.TotalCount ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authors Controller => Exception occurred while loading more authors via cursor");
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
