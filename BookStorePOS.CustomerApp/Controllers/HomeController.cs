using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookStorePOS.CustomerApp.Models;
using BookStorePOS.Shared.Models.Book;
using BookStorePOS.Shared.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");

                // Request trending books (last 7 days, top 3 for 3D fan stack) and bestsellers (last 30 days, top 4 for shelf grid)
                var trendingTask = client.GetAsync("api/book/trending?days=7&limit=3");
                var bestsellersTask = client.GetAsync("api/book/bestsellers?days=30&limit=4");

                await Task.WhenAll(trendingTask, bestsellersTask);

                var trendingResp = await trendingTask;
                var bestsellersResp = await bestsellersTask;

                List<BookModel> trendingBooks = new();
                if (trendingResp.IsSuccessStatusCode)
                {
                    var trendingJson = await trendingResp.Content.ReadAsStringAsync();
                    var trendingResult = JsonConvert.DeserializeObject<Result<List<BookModel>>>(trendingJson);
                    if (trendingResult?.Data != null)
                    {
                        trendingBooks = trendingResult.Data;
                    }
                }

                List<BookModel> bestsellerBooks = new();
                if (bestsellersResp.IsSuccessStatusCode)
                {
                    var bestsellersJson = await bestsellersResp.Content.ReadAsStringAsync();
                    var bestsellersResult = JsonConvert.DeserializeObject<Result<List<BookModel>>>(bestsellersJson);
                    if (bestsellersResult?.Data != null)
                    {
                        bestsellerBooks = bestsellersResult.Data;
                    }
                }

                var viewModel = new HomeViewModel
                {
                    TrendingBooks = trendingBooks,
                    BestsellerBooks = bestsellerBooks
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index => Exception occurred");
                return View(new HomeViewModel());
            }
        }

        [ActionName("NotFound")]
        public IActionResult PageNotFound()
        {
            try
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotFound => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
