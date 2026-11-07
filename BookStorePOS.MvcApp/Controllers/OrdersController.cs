using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;
using Newtonsoft.Json;

namespace BookStorePOS.MvcApp.Controllers
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

        [ActionName("Index")]
        public async Task<IActionResult> OrdersAsync(OrderHistoryRequestModel requestModel)
        {
            var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
            var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
            if (!hasToken && !hasRefresh)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = "/Orders" });
            }

            var userRole = Request.Cookies["pos_role"] ?? "";
            var userPerms = Request.Cookies["pos_permissions"] ?? "";
            bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
            var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!isAdmin)
            {
                if (!permList.Contains("Order.View", StringComparer.OrdinalIgnoreCase))
                {
                    if (permList.Contains("Order.Create", StringComparer.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Checkout");
                    }
                    if (permList.Contains("Dashboard.View", StringComparer.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    return RedirectToAction("Index", "Checkout");
                }
            }

            bool canConfirm = isAdmin || permList.Contains("Order.Confirm", StringComparer.OrdinalIgnoreCase);
            bool canReject = isAdmin || permList.Contains("Order.Reject", StringComparer.OrdinalIgnoreCase);
            ViewData["CanConfirmOrders"] = canConfirm;
            ViewData["CanRejectOrders"] = canReject;

            try
            {
                _logger.LogInformation("Orders Async => Fetching order history");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}";
                if (requestModel.StartDate.HasValue) qs += $"&StartDate={requestModel.StartDate.Value:yyyy-MM-dd}";
                if (requestModel.EndDate.HasValue) qs += $"&EndDate={requestModel.EndDate.Value:yyyy-MM-dd}";
                if (!string.IsNullOrWhiteSpace(requestModel.OrderSource)) qs += $"&OrderSource={Uri.EscapeDataString(requestModel.OrderSource)}";

                var httpResponse = await client.GetAsync($"api/order/history{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderHistoryResponseModel>(jsonString);

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    ViewData["Orders"] = response.Data.Orders;
                    ViewData["Summary"] = response.Data.Summary;
                    ViewData["TotalPages"] = response.Data.TotalPages;
                    ViewData["CurrentPage"] = response.Data.Page;
                    ViewData["StartDate"] = requestModel.StartDate?.ToString("yyyy-MM-dd");
                    ViewData["EndDate"] = requestModel.EndDate?.ToString("yyyy-MM-dd");
                    ViewData["SelectedSource"] = string.IsNullOrWhiteSpace(requestModel.OrderSource) ? "All" : requestModel.OrderSource;
                    _logger.LogInformation("Orders Async => Order history fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"Orders Async => Failed to fetch order history: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                }

                return View("Order");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrdersAsync => Exception occurred");
            return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("OrderDetail")]
        public async Task<IActionResult> OrderDetailAsync(int id)
        {
            var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
            var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
            if (!hasToken && !hasRefresh)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = $"/Orders/OrderDetail/{id}" });
            }

            try
            {
                if (id <= 0 || !ModelState.IsValid)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Order Detail Async => Fetching order details for OrderId: {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/order/{id}");

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    return RedirectToAction("Login", "Auth", new { returnUrl = $"/Orders/OrderDetail/{id}" });
                }

                if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning($"Order Detail Async => Access denied for OrderId: {id}");
                    TempData["Message"] = "You do not have permission to view this order.";
                    TempData["isSuccess"] = false;
                    return RedirectToAction("Index", "Checkout");
                }

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderGetByIdResponseModel>(jsonString);

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    _logger.LogInformation($"Order Detail Async => Order details fetched successfully for OrderId: {id}");
                    ViewData["OrderDetail"] = response.Data;
                    return View("OrderDetail");
                }
                else
                {
                    _logger.LogWarning($"Order Detail Async => Failed to fetch order details for OrderId: {id}. Message: {response?.Message}");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderDetailAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet]
        [ActionName("Detail")]
        public async Task<IActionResult> OrderJsonDetailAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = "Invalid order ID." });
                }

                _logger.LogInformation($"Order Json Detail Async => Fetching order details for OrderId: {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/order/{id}");

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return Json(new { isSuccess = false, message = "Order not found." });
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderGetByIdResponseModel>(jsonString);

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    return Json(new { isSuccess = true, data = response.Data });
                }

                return Json(new { isSuccess = false, message = response?.Message ?? "Order not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderJsonDetailAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> OrderCreateAsync([FromBody] OrderCreateRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation("Order Create Async => Creating new order");

                if (requestModel == null || requestModel.Items == null || requestModel.Items.Count == 0)
                {
                    _logger.LogWarning("Order Create Async => No items provided");
                    return Json(new OrderCreateResponseModel { IsSuccess = false, Message = "Please provide at least one item." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
                var httpResponse = await client.PostAsync("api/order", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderCreateResponseModel>(jsonString) ?? new OrderCreateResponseModel { IsSuccess = false, Message = "Unknown error" };
            
                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation("Order Create Async => Order created successfully");
                }
                else
                {
                    _logger.LogWarning($"Order Create Async => Failed to create order: {response?.Message}");
                }

                return Json(response);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrderCreateAsync => Exception occurred");
            return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
    }
}
