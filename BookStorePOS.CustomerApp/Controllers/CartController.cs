using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Shared.Models.Auth;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;
using Newtonsoft.Json;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpClientFactory httpClientFactory, ILogger<CartController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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

                        _logger.LogInformation("CartController => Token auto-refreshed successfully for user {User}", model.Username);
                        return model.AccessToken;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CartController => Failed to auto-refresh token");
            }

            return null;
        }

        private bool IsCustomerLoggedIn()
        {
            return !string.IsNullOrEmpty(Request.Cookies["customer_token"]) ||
                   !string.IsNullOrEmpty(Request.Cookies["customer_refresh"]);
        }

        /// <summary>
        /// Shopping Cart Page
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public IActionResult Cart()
        {
            _logger.LogInformation("Cart => Rendering shopping cart page");
            return View("Cart");
        }

        /// <summary>
        /// Dedicated Checkout Page
        /// </summary>
        [HttpGet]
        [ActionName("Checkout")]
        public IActionResult Checkout()
        {
            _logger.LogInformation("Checkout => Rendering checkout page");

            if (!IsCustomerLoggedIn())
            {
                TempData["Message"] = "Please sign in before proceeding to checkout.";
                TempData["isSuccess"] = false;
                return Redirect("/Auth/Login?returnUrl=/Cart/Checkout");
            }

            // Provide pre-fill info from cookie if present
            string? customerUser = Request.Cookies["customer_user"];
            if (!string.IsNullOrEmpty(customerUser))
            {
                try { customerUser = Uri.UnescapeDataString(customerUser); } catch { }
            }

            ViewData["PreFillName"] = customerUser ?? "";
            return View("Checkout");
        }

        /// <summary>
        /// Legacy Payment redirect to order Success receipt
        /// </summary>
        [HttpGet]
        [ActionName("Payment")]
        public IActionResult Payment(int id)
        {
            return RedirectToAction("Success", new { id });
        }

        /// <summary>
        /// Dedicated Order Confirmation / Success Receipt Page
        /// GET: /Cart/Success/{id}
        /// </summary>
        [HttpGet]
        [ActionName("Success")]
        public async Task<IActionResult> SuccessAsync(int id)
        {
            if (id <= 0)
            {
                return Redirect("/Cart");
            }

            _logger.LogInformation("Success => Rendering order success receipt for order #{OrderId}", id);

            try
            {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var token = await GetOrRefreshCustomerTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var httpResponse = await client.GetAsync($"api/order/{id}");
                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        var response = JsonConvert.DeserializeObject<OrderGetByIdResponseModel>(jsonString);
                        if (response != null && response.IsSuccess && response.Data != null)
                        {
                            ViewData["Order"] = response.Data;
                            return View("Success");
                        }
                    }
                }

                ViewData["OrderId"] = id;
                return View("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Success => Failed to load order #{OrderId}", id);
                ViewData["OrderId"] = id;
                return View("Success");
            }
        }

        /// <summary>
        /// AJAX Action: Submit Online Order to WebAPI
        /// Supports FormData with screenshot file or JSON
        /// POST: /Cart/CreateOnlineOrder
        /// </summary>
        [HttpPost]
        [ActionName("CreateOnlineOrder")]
        public async Task<IActionResult> CreateOnlineOrderAsync()
        {
            try
            {
                if (!IsCustomerLoggedIn())
                {
                    return Json(new { isSuccess = false, message = "Please sign in to place your order." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var token = await GetOrRefreshCustomerTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage httpResponse;

                if (Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    using var formContent = new MultipartFormDataContent();

                    foreach (var key in form.Keys)
                    {
                        formContent.Add(new StringContent(form[key].ToString()), key);
                    }

                    if (form.Files.Count > 0 && form.Files[0].Length > 0)
                    {
                        var file = form.Files[0];
                        var stream = file.OpenReadStream();
                        var streamContent = new StreamContent(stream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
                        formContent.Add(streamContent, "file", file.FileName);
                    }

                    httpResponse = await client.PostAsync("api/order/online", formContent);
                }
                else
                {
                    using var reader = new StreamReader(Request.Body);
                    var rawBody = await reader.ReadToEndAsync();
                    using var content = new StringContent(rawBody, Encoding.UTF8, "application/json");
                    httpResponse = await client.PostAsync("api/order/online", content);
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                OrderCreateResponseModel? response = null;

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    try
                    {
                        response = JsonConvert.DeserializeObject<OrderCreateResponseModel>(jsonString);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize WebAPI response: {Raw}", jsonString);
                    }
                }

                if (response == null)
                {
                    string fallbackMsg = !httpResponse.IsSuccessStatusCode
                        ? $"Server error ({(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}). Please check your connection or try again."
                        : "Unknown response from server.";

                    response = new OrderCreateResponseModel
                    {
                        IsSuccess = false,
                        Message = fallbackMsg
                    };
                }

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Create Online Order => Order created successfully with OrderId {OrderId}", response.Data?.OrderId);
                }
                else
                {
                    _logger.LogWarning("Create Online Order => Order failed: {Message}", response.Message);
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Online Order => Exception occurred");
                return StatusCode(500, new { isSuccess = false, message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
