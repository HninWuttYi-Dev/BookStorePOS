using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;
using Newtonsoft.Json;

namespace BookStorePOS.MvcApp.Controllers
{
    public class SalesReportsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SalesReportsController> _logger;

        public SalesReportsController(IHttpClientFactory httpClientFactory, ILogger<SalesReportsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(string? source = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int limit = 10)
        {
            try
            {
                var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
                var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
                if (!hasToken && !hasRefresh)
                {
                    _logger.LogInformation("SalesReports => No auth cookies found, redirecting to Login");
                    return RedirectToLogin();
                }

                var userRole = Request.Cookies["pos_role"] ?? "";
                var userPerms = Request.Cookies["pos_permissions"] ?? "";
                bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
                bool isCashier = string.Equals(userRole, "Cashier", StringComparison.OrdinalIgnoreCase);

                if (!isAdmin)
                {
                    _logger.LogWarning("SalesReports => Non-admin user '{User}' attempted to access SalesReports. Access denied.", Request.Cookies["pos_user"]);
                    if (isCashier)
                    {
                        return RedirectToAction("Index", "Checkout");
                    }
                    var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (permList.Contains("Order.Create", StringComparer.OrdinalIgnoreCase))
                    {
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
                    return RedirectToAction("Index", "Checkout");
                }

                _logger.LogInformation("SalesReports => Fetching sales report (Completed orders)");

                var client = _httpClientFactory.CreateClient("WebAPI");

                // Sales report only displays Completed orders (OrderStatus = 2)
                var qs = $"?Page={page}&Limit={limit}&OrderStatus=2";
                if (!string.IsNullOrWhiteSpace(source) && !source.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    qs += $"&OrderSource={Uri.EscapeDataString(source)}";
                }
                if (startDate.HasValue)
                {
                    qs += $"&StartDate={startDate.Value:yyyy-MM-dd}";
                }
                if (endDate.HasValue)
                {
                    qs += $"&EndDate={endDate.Value:yyyy-MM-dd}";
                }

                var httpResponse = await client.GetAsync($"api/order{qs}");
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    _logger.LogWarning("SalesReports => Unauthorized / Session expired, redirecting to Login");
                    return RedirectToLogin();
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderListResponseModel>(jsonString);

                var orders = response?.Data ?? new List<OrderModel>();
                var totalCount = response?.TotalCount ?? 0;
                var totalRevenue = response?.TotalRevenue ?? 0m;
                var totalPages = response?.TotalPages ?? 1;

                ViewData["Orders"] = orders;
                ViewData["TotalCount"] = totalCount;
                ViewData["TotalRevenue"] = totalRevenue;
                ViewData["AverageOrderValue"] = totalCount > 0 ? totalRevenue / totalCount : 0m;
                ViewData["CurrentPage"] = response?.Page ?? page;
                ViewData["TotalPages"] = totalPages == 0 ? 1 : totalPages;
                ViewData["Limit"] = limit;
                ViewData["SelectedSource"] = string.IsNullOrWhiteSpace(source) ? "All" : source;
                ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
                ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SalesReports Index => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        private IActionResult RedirectToLogin()
        {
            var returnUrl = Request.Path + Request.QueryString;
            return Redirect($"/Auth/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }
}

