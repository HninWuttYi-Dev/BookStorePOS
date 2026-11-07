using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using BookStorePOS.Shared.Models.Book;
using BookStorePOS.Shared.Models.Order;
using Newtonsoft.Json;

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
                // 1. Session check: if neither token nor refresh token is in cookies, bounce to login
                var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
                var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
                if (!hasToken && !hasRefresh)
                {
                    _logger.LogInformation("DashboardIndexAsync => No auth cookies found, redirecting to Login");
                    return RedirectToLogin();
                }

                // 2. Permission check: if user cannot view dashboard, route to permitted module
                var userRole = Request.Cookies["pos_role"] ?? "";
                var userPerms = Request.Cookies["pos_permissions"] ?? "";
                bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

                if (!isAdmin)
                {
                    var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (!permList.Contains("Dashboard.View", StringComparer.OrdinalIgnoreCase))
                    {
                        if (permList.Contains("Order.Create", StringComparer.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("DashboardIndexAsync => User without Dashboard.View routed to Checkout");
                            return RedirectToAction("Index", "Checkout");
                        }
                        else if (permList.Contains("Book.View", StringComparer.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Index", "Book");
                        }
                        else if (permList.Contains("Order.View", StringComparer.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Index", "Orders");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Checkout");
                        }
                    }
                }

                _logger.LogInformation("Dashboard Index Async => Fetching dashboard data");

                var client = _httpClientFactory.CreateClient("WebAPI");

                // 2) Summary
                var summaryHttp = await client.GetAsync("api/order/summary");
                if (summaryHttp.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    _logger.LogWarning("DashboardIndexAsync => Order summary unauthorized / session expired, redirecting to Login");
                    return RedirectToLogin();
                }

                var summaryJson = await summaryHttp.Content.ReadAsStringAsync();
                var summaryResponse = JsonConvert.DeserializeObject<OrderSummaryResponseModel>(summaryJson);

                if (summaryResponse != null && summaryResponse.IsSuccess && summaryResponse.Data != null)
                {
                    ViewData["TodaySales"] = summaryResponse.Data.todayTotalRevenue;
                    ViewData["ThisMonthSales"] = summaryResponse.Data.thisMonthTotalRevenue;
                    ViewData["PendingOnlineOrdersCount"] = summaryResponse.Data.pendingOnlineOrderCount;
                    ViewData["TodayGrowthPercentage"] = summaryResponse.Data.todayGrowthPercentage;
                    ViewData["MonthGrowthPercentage"] = summaryResponse.Data.monthGrowthPercentage;
                }
                else
                {
                    ViewData["TodaySales"] = 0m;
                    ViewData["ThisMonthSales"] = 0m;
                    ViewData["PendingOnlineOrdersCount"] = 0;
                    ViewData["TodayGrowthPercentage"] = 0m;
                    ViewData["MonthGrowthPercentage"] = 0m;
                    ViewData["isSuccess"] = false;
                    ViewData["Message"] = summaryResponse?.Message;
                }

                // 3) Low stock
                var lowStockHttp = await client.GetAsync("api/book/low-stock");
                if (lowStockHttp.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    _logger.LogWarning("DashboardIndexAsync => Low stock unauthorized / session expired, redirecting to Login");
                    return RedirectToLogin();
                }

                var lowStockJson = await lowStockHttp.Content.ReadAsStringAsync();
                var lowStockResponse = JsonConvert.DeserializeObject<BookListResponseModel>(lowStockJson);

                var lowStockBooks = lowStockResponse != null && lowStockResponse.IsSuccess && lowStockResponse.Data != null
                    ? lowStockResponse.Data
                    : new List<BookModel>();

                ViewData["LowStockItemsCount"] = lowStockBooks.Count;
                ViewData["LowStockBooks"] = lowStockBooks.Take(5).ToList();

                // 4) Recent orders
                var historyHttp = await client.GetAsync("api/order/history?Page=1&Limit=5");
                if (historyHttp.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    _logger.LogWarning("DashboardIndexAsync => Order history unauthorized / session expired, redirecting to Login");
                    return RedirectToLogin();
                }

                var historyJson = await historyHttp.Content.ReadAsStringAsync();
                var historyResponse = JsonConvert.DeserializeObject<OrderHistoryResponseModel>(historyJson);

                var recentOrders = historyResponse != null && historyResponse.IsSuccess && historyResponse.Data?.Orders != null
                    ? historyResponse.Data.Orders
                    : new List<OrderModel>();

                ViewData["RecentOrders"] = recentOrders;

                // 5) Revenue Chart Data (ApexCharts)
                var chartHttp = await client.GetAsync("api/order/revenue-chart");
                SalesRevenueChartDataModel chartData = new();
                if (chartHttp.IsSuccessStatusCode)
                {
                    var chartJson = await chartHttp.Content.ReadAsStringAsync();
                    var chartResp = JsonConvert.DeserializeObject<SalesRevenueChartResponseModel>(chartJson);
                    if (chartResp != null && chartResp.IsSuccess && chartResp.Data != null)
                    {
                        chartData = chartResp.Data;
                    }
                }
                ViewData["ChartDataJson"] = JsonConvert.SerializeObject(chartData);

                // 6) Top Sellers (Today / 7 Days / 30 Days)
                var topSellersHttp = await client.GetAsync("api/order/top-sellers-dashboard?limit=5");
                DashboardTopSellersDataModel topSellersData = new();
                if (topSellersHttp.IsSuccessStatusCode)
                {
                    var topSellersJson = await topSellersHttp.Content.ReadAsStringAsync();
                    var topSellersResp = JsonConvert.DeserializeObject<DashboardTopSellersResponseModel>(topSellersJson);
                    if (topSellersResp != null && topSellersResp.IsSuccess && topSellersResp.Data != null)
                    {
                        topSellersData = topSellersResp.Data;
                    }
                }
                else
                {
                    var legacyHttp = await client.GetAsync("api/order/top-sellers?Limit=5&Period=7days");
                    if (legacyHttp.IsSuccessStatusCode)
                    {
                        var legJson = await legacyHttp.Content.ReadAsStringAsync();
                        var legResp = JsonConvert.DeserializeObject<TopSellersResponseModel>(legJson);
                        if (legResp != null && legResp.IsSuccess && legResp.Data != null)
                        {
                            topSellersData.SevenDays = legResp.Data;
                        }
                    }
                }
                ViewData["TopSellersDataJson"] = JsonConvert.SerializeObject(topSellersData);
                ViewData["TopSellers"] = topSellersData.SevenDays;

                _logger.LogInformation("Dashboard Index Async => Dashboard data fetched successfully");
                return View("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DashboardIndexAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        private IActionResult RedirectToLogin()
        {
            Response.Cookies.Delete("pos_token");
            Response.Cookies.Delete("pos_refresh");
            Response.Cookies.Delete("pos_user");
            Response.Cookies.Delete("pos_role");
            Response.Cookies.Delete("pos_permissions");
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Dashboard" });
        }
    }
}
