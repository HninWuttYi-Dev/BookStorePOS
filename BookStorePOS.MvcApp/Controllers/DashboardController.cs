using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BookStorePOS.MvcApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        [ActionName("Index")]
        public async Task<IActionResult> DashboardIndexAsync()
        {
        try
        {
                _logger.LogInformation("Dashboard Index Async => Fetching dashboard data");

                var client = _httpClientFactory.CreateClient("WebAPI");

                // 1) Summary
                var summaryHttp = await client.GetAsync("api/order/summary");
                var summaryJson = await summaryHttp.Content.ReadAsStringAsync();
                var summaryResponse = JsonConvert.DeserializeObject<OrderSummaryResponseModel>(summaryJson);

                if (summaryResponse != null && summaryResponse.isSuccess && summaryResponse.Data != null)
                {
                    ViewData["TodaySales"] = summaryResponse.Data.todayTotalRevenue;
                    ViewData["ThisMonthSales"] = summaryResponse.Data.thisMonthTotalRevenue;
                }
                else
                {
                    ViewData["TodaySales"] = 0m;
                    ViewData["ThisMonthSales"] = 0m;
                    ViewData["isSuccess"] = false;
                    ViewData["Message"] = summaryResponse?.Message;
                }

                // 2) Low stock
                var lowStockHttp = await client.GetAsync("api/book/low-stock");
                var lowStockJson = await lowStockHttp.Content.ReadAsStringAsync();
                var lowStockResponse = JsonConvert.DeserializeObject<BookListResponseModel>(lowStockJson);

                var lowStockBooks = lowStockResponse != null && lowStockResponse.isSuccess && lowStockResponse.Data != null
                    ? lowStockResponse.Data
                    : new List<BookModel>();

                ViewData["LowStockItemsCount"] = lowStockBooks.Count;
                ViewData["LowStockBooks"] = lowStockBooks.Take(5).ToList();

                // 3) Recent orders
                var historyHttp = await client.GetAsync("api/order/history?Page=1&Limit=5");
                var historyJson = await historyHttp.Content.ReadAsStringAsync();
                var historyResponse = JsonConvert.DeserializeObject<OrderHistoryResponseModel>(historyJson);

                var recentOrders = historyResponse != null && historyResponse.isSuccess && historyResponse.Data?.Orders != null
                    ? historyResponse.Data.Orders
                    : new List<OrderModel>();

                ViewData["RecentOrders"] = recentOrders;

                _logger.LogInformation("Dashboard Index Async => Dashboard data fetched successfully");

                // Optional: pass one response model if you want @model
                return View("Dashboard");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DashboardIndexAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
    }
}
