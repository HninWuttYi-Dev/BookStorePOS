using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Auth;
using BookStorePOS.Shared.Models.Order;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private bool IsCustomerLoggedIn()
        {
            return !string.IsNullOrEmpty(Request.Cookies["customer_token"]) ||
                   !string.IsNullOrEmpty(Request.Cookies["customer_refresh"]);
        }

        private async Task<string?> GetOrRefreshCustomerTokenAsync()
        {
            var token = Request.Cookies["customer_token"];
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            var refreshToken = Request.Cookies["customer_refresh"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var request = new RefreshTokenRequestModel { RefreshToken = refreshToken };
                var json = JsonConvert.SerializeObject(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/refresh", content);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<RefreshTokenResponseModel>(jsonString);
                    if (model != null && model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
                    {
                        Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                        });

                        if (!string.IsNullOrEmpty(model.RefreshToken))
                        {
                            Response.Cookies.Append("customer_refresh", model.RefreshToken, new CookieOptions
                            {
                                HttpOnly = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTimeOffset.UtcNow.AddDays(7)
                            });
                        }

                        _logger.LogInformation("OrdersController => Token auto-refreshed successfully");
                        return model.AccessToken;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OrdersController => Failed to auto-refresh token");
            }

            return null;
        }

        /// <summary>
        /// Customer Order History List: /Orders
        /// Authenticated customers only. 10 orders per page.
        /// </summary>
        [ActionName("Index")]
        public async Task<IActionResult> OrderListAsync(string? status = null, int page = 1)
        {
            if (!IsCustomerLoggedIn())
            {
                _logger.LogInformation("Orders => Guest attempted to view order history. Redirecting to login.");
                return Redirect("/Auth/Login?returnUrl=/Orders");
            }

            if (page < 1) page = 1;
            const int limit = 10;

            try
            {
                var token = await GetOrRefreshCustomerTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return Redirect("/Auth/Login?returnUrl=/Orders");
                }

                _logger.LogInformation("Orders => Fetching customer online order history page {Page}", page);
                var client = _httpClientFactory.CreateClient("WebAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string qs = $"?OrderSource=Online&Page={page}&Limit={limit}";
                if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
                {
                    qs += $"&OrderStatus={status}";
                }

                var httpResponse = await client.GetAsync($"api/order{qs}");
                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<OrderListResponseModel>(jsonString);

                    ViewData["Orders"] = response?.Data ?? new List<OrderModel>();
                    ViewData["ActiveStatus"] = string.IsNullOrWhiteSpace(status) ? "All" : status;
                    ViewData["CurrentPage"] = response?.Page > 0 ? response.Page : page;
                    ViewData["TotalPages"] = response?.TotalPages ?? 1;
                    ViewData["TotalCount"] = response?.TotalCount ?? 0;
                    ViewData["PendingCount"] = response?.PendingCount ?? 0;
                    ViewData["CompletedCount"] = response?.CompletedCount ?? 0;
                    ViewData["RejectedCount"] = response?.RejectedCount ?? 0;
                    return View("Index");
                }

                _logger.LogWarning("Orders => WebAPI returned {StatusCode} while fetching order history", httpResponse.StatusCode);
                ViewData["Orders"] = new List<OrderModel>();
                ViewData["ActiveStatus"] = "All";
                ViewData["CurrentPage"] = 1;
                ViewData["TotalPages"] = 1;
                ViewData["TotalCount"] = 0;
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Orders => Exception occurred while fetching order history");
                ViewData["Orders"] = new List<OrderModel>();
                ViewData["ActiveStatus"] = "All";
                ViewData["CurrentPage"] = 1;
                ViewData["TotalPages"] = 1;
                ViewData["TotalCount"] = 0;
                return View("Index");
            }
        }

        /// <summary>
        /// Customer Order Detail & Receipt: /Orders/Detail/{id}
        /// Authenticated customers only.
        /// </summary>
        [ActionName("Detail")]
        public async Task<IActionResult> OrderDetailAsync(int id)
        {
            if (!IsCustomerLoggedIn())
            {
                return Redirect($"/Auth/Login?returnUrl=/Orders/Detail/{id}");
            }

            if (id <= 0)
            {
                return Redirect("/Orders");
            }

            try
            {
                var token = await GetOrRefreshCustomerTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return Redirect($"/Auth/Login?returnUrl=/Orders/Detail/{id}");
                }

                _logger.LogInformation("OrderDetail => Fetching order #{OrderId} for customer", id);
                var client = _httpClientFactory.CreateClient("WebAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var httpResponse = await client.GetAsync($"api/order/{id}");
                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<OrderGetByIdResponseModel>(jsonString);

                    if (response != null && response.IsSuccess && response.Data != null)
                    {
                        ViewData["Order"] = response.Data;
                        return View("Detail");
                    }
                }

                _logger.LogWarning("OrderDetail => Order #{OrderId} not found or unauthorized", id);
                return Redirect("/Orders");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderDetail => Exception occurred for order #{OrderId}", id);
                return Redirect("/Orders");
            }
        }
    }
}

