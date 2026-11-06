using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Genre;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class GenresController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GenresController> _logger;

        public GenresController(IHttpClientFactory httpClientFactory, ILogger<GenresController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index(string search = "", int limit = 6)
        {
            try
            {
                _logger.LogInformation("Genres Controller => Fetching initial genre shelves via cursor, search: {Search}", search);

                var client = _httpClientFactory.CreateClient("WebAPI");
                var encodedSearch = Uri.EscapeDataString(search ?? "");
                var httpResponse = await client.GetAsync($"api/genre/cursor?GenreName={encodedSearch}&Limit={limit}");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<GenreCursorResponseModel>(jsonString);

                var genres = response?.Data ?? new List<GenreModel>();

                ViewData["Genres"] = genres;
                ViewData["Search"] = search;
                ViewData["NextCursor"] = response?.NextCursor;
                ViewData["HasNextPage"] = response?.HasNextPage ?? false;
                ViewData["TotalCount"] = response?.TotalCount ?? genres.Count;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Genres Controller => Exception occurred while fetching initial genres");
                ViewData["Genres"] = new List<GenreModel>();
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
                _logger.LogInformation("Genres Controller => Loading more genre shelves with cursor: {Cursor}", cursor);

                var client = _httpClientFactory.CreateClient("WebAPI");
                var encodedSearch = Uri.EscapeDataString(search ?? "");
                var cursorParam = cursor.HasValue ? $"&Cursor={cursor.Value}" : "";
                var httpResponse = await client.GetAsync($"api/genre/cursor?GenreName={encodedSearch}&Limit={limit}{cursorParam}");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<GenreCursorResponseModel>(jsonString);

                return Json(new
                {
                    isSuccess = response?.IsSuccess ?? false,
                    data = response?.Data ?? new List<GenreModel>(),
                    nextCursor = response?.NextCursor,
                    hasNextPage = response?.HasNextPage ?? false,
                    totalCount = response?.TotalCount ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Genres Controller => Exception occurred while loading more genres via cursor");
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
