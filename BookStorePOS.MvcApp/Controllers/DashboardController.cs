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
        private readonly IOrderService _orderService;
        private readonly IBookService _bookService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IOrderService orderService, IBookService bookService, ILogger<DashboardController> logger)
        {
            _orderService = orderService;
            _bookService = bookService;
            _logger = logger;
        }
        [ActionName("Index")]
        public async Task<IActionResult> DashboardIndexAsync()
        {
            _logger.LogInformation("Dashboard Index Async => Fetching dashboard data");

            // 1) Summary
            var summaryResponse = await _orderService.GetOrderSummaryAsync();
            if (summaryResponse.isSuccess && summaryResponse.Data != null)
            {
                ViewData["TodaySales"] = summaryResponse.Data.todayTotalRevenue;
                ViewData["ThisMonthSales"] = summaryResponse.Data.thisMonthTotalRevenue;
            }
            else
            {
                ViewData["TodaySales"] = 0m;
                ViewData["ThisMonthSales"] = 0m;
                ViewData["isSuccess"] = false;
                ViewData["Message"] = summaryResponse.Message;
            }

            // 2) Low stock
            var lowStockResponse = await _bookService.GetLowStockBooksAsync();
            var lowStockBooks = lowStockResponse.isSuccess && lowStockResponse.Data != null
                ? lowStockResponse.Data
                : new List<BookModel>();

            ViewData["LowStockItemsCount"] = lowStockBooks.Count;
            ViewData["LowStockBooks"] = lowStockBooks.Take(5).ToList();

            // 3) Recent orders
            var historyResponse = await _orderService.GetOrderHistoryAsync(
                new OrderHistoryRequestModel { Page = 1, Limit = 5 });

            var recentOrders = historyResponse.isSuccess && historyResponse.Data?.Orders != null
                ? historyResponse.Data.Orders
                : new List<OrderModel>();

            ViewData["RecentOrders"] = recentOrders;

            _logger.LogInformation("Dashboard Index Async => Dashboard data fetched successfully");

            // Optional: pass one response model if you want @model
            return View("Dashboard");
        }
    }
}
