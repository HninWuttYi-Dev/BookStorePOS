using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OnlineOrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OnlineOrdersController> _logger;

        public OnlineOrdersController(IHttpClientFactory httpClientFactory, ILogger<OnlineOrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> OnlineOrderListAsync(string? tab = "Pending", string? search = "", int page = 1)
        {
            try
            {
                var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
                var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
                if (!hasToken && !hasRefresh)
                {
                    _logger.LogInformation("OnlineOrderListAsync => No auth cookies found, redirecting to Login");
                    return RedirectToLogin();
                }

                var userRole = Request.Cookies["pos_role"] ?? "";
                var userPerms = Request.Cookies["pos_permissions"] ?? "";
                bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

                if (!isAdmin)
                {
                    var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    bool canView = permList.Contains("OnlineOrder.View", StringComparer.OrdinalIgnoreCase) || permList.Contains("Order.View", StringComparer.OrdinalIgnoreCase);
                    if (!canView)
                    {
                        _logger.LogWarning("OnlineOrderListAsync => Access denied for user '{User}' without OnlineOrder.View", Request.Cookies["pos_user"]);
                        TempData["Message"] = "Access Denied: You do not have permission to view Online Orders.";
                        TempData["isSuccess"] = false;
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                bool canProcess = isAdmin || userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains("OnlineOrder.Update", StringComparer.OrdinalIgnoreCase);
                ViewData["CanProcessOrders"] = canProcess;

                _logger.LogInformation("Online Order List Async => Fetching online orders");

                tab = string.IsNullOrWhiteSpace(tab) ? "Pending" : tab;
                page = page <= 0 ? 1 : page;
                int limit = 10; // Exactly 10 per page, consistent with Orders

                var qs = $"?OrderSource=Online&Page={page}&Limit={limit}";
                if (tab == "Pending") qs += "&OrderStatus=Pending";
                else if (tab == "Completed") qs += "&OrderStatus=Completed";
                else if (tab == "Rejected") qs += "&OrderStatus=Rejected";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    qs += $"&Search={Uri.EscapeDataString(search.Trim())}";
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/order{qs}");

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
                {
                    _logger.LogWarning("OnlineOrderListAsync => Unauthorized access, redirecting to Login");
                    return RedirectToLogin();
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OrderListResponseModel>(jsonString);

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    ViewData["Orders"] = response.Data;
                    ViewData["CurrentTab"] = tab;
                    ViewData["Search"] = search ?? "";
                    ViewData["PendingCount"] = response.PendingCount;
                    ViewData["CompletedCount"] = response.CompletedCount;
                    ViewData["RejectedCount"] = response.RejectedCount;
                    ViewData["TotalRevenue"] = response.TotalRevenue;
                    ViewData["CurrentPage"] = response.Page > 0 ? response.Page : page;
                    ViewData["TotalPages"] = response.TotalPages > 0 ? response.TotalPages : 1;
                    ViewData["TotalCount"] = response.TotalCount;

                    _logger.LogInformation("Online Order List Async => Online orders fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"Online Order List Async => Failed to fetch online orders: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                    ViewData["Orders"] = new List<OrderModel>();
                    ViewData["CurrentTab"] = tab;
                    ViewData["Search"] = search ?? "";
                    ViewData["CurrentPage"] = page;
                    ViewData["TotalPages"] = 1;
                    ViewData["TotalCount"] = 0;
                }

                return View("OnlineOrderList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnlineOrderListAsync => Exception occurred");
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
            return RedirectToAction("Login", "Auth", new { returnUrl = "/OnlineOrders" });
        }

        [HttpGet]
        [ActionName("Detail")]
        public async Task<IActionResult> OnlineOrderDetailAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = "Invalid order ID." });
                }

                _logger.LogInformation($"Online Order Detail Async => Fetching order details for OrderId: {id}");

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
                    _logger.LogInformation($"Online Order Detail Async => Order details fetched successfully for OrderId: {id}");
                    return Json(new { isSuccess = true, data = response.Data });
                }

                _logger.LogWarning($"Online Order Detail Async => Failed to fetch order {id}: {response?.Message}");
                return Json(new { isSuccess = false, message = response?.Message ?? "Order not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnlineOrderDetailAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> OnlineOrderConfirmAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = "Invalid order ID." });
                }

                var userRole = Request.Cookies["pos_role"] ?? "";
                var userPerms = Request.Cookies["pos_permissions"] ?? "";
                bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
                bool canUpdate = isAdmin || userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains("OnlineOrder.Update", StringComparer.OrdinalIgnoreCase);

                if (!canUpdate)
                {
                    _logger.LogWarning("OnlineOrderConfirmAsync => Access denied for user '{User}' without OnlineOrder.Update", Request.Cookies["pos_user"]);
                    return Json(new { isSuccess = false, message = "Access Denied: You do not have permission to verify or confirm online orders." });
                }

                _logger.LogInformation($"Online Order Confirm Async => Confirming order {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                StringContent stringContent = new StringContent("{}", Encoding.UTF8, Application.Json);
                var httpResponse = await client.PostAsync($"api/order/{id}/confirm", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Result<OrderModel>>(jsonString);

                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation($"Online Order Confirm Async => Order {id} confirmed successfully");
                    return Json(new { isSuccess = true, message = response.Message ?? "Order payment verified and confirmed successfully." });
                }

                _logger.LogWarning($"Online Order Confirm Async => Failed to confirm order {id}: {response?.Message}");
                return Json(new { isSuccess = false, message = response?.Message ?? "Failed to confirm order." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnlineOrderConfirmAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Reject")]
        public async Task<IActionResult> OnlineOrderRejectAsync(int id, [FromBody] OrderRejectRequestModel requestModel)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = "Invalid order ID." });
                }

                var userRole = Request.Cookies["pos_role"] ?? "";
                var userPerms = Request.Cookies["pos_permissions"] ?? "";
                bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
                bool canUpdate = isAdmin || userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains("OnlineOrder.Update", StringComparer.OrdinalIgnoreCase);

                if (!canUpdate)
                {
                    _logger.LogWarning("OnlineOrderRejectAsync => Access denied for user '{User}' without OnlineOrder.Update", Request.Cookies["pos_user"]);
                    return Json(new { isSuccess = false, message = "Access Denied: You do not have permission to reject online orders." });
                }

                if (string.IsNullOrWhiteSpace(requestModel?.RejectReason))
                {
                    _logger.LogWarning($"Online Order Reject Async => Reject reason is required for OrderId: {id}");
                    return Json(new { isSuccess = false, message = "Please provide a reason for rejecting the order." });
                }

                _logger.LogInformation($"Online Order Reject Async => Rejecting order {id} with reason: {requestModel.RejectReason}");

                requestModel.OrderId = id;
                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);

                var httpResponse = await client.PostAsync($"api/order/{id}/reject", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Result<OrderModel>>(jsonString);

                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation($"Online Order Reject Async => Order {id} rejected successfully");
                    return Json(new { isSuccess = true, message = response.Message ?? "Order rejected successfully." });
                }

                _logger.LogWarning($"Online Order Reject Async => Failed to reject order {id}: {response?.Message}");
                return Json(new { isSuccess = false, message = response?.Message ?? "Failed to reject order." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnlineOrderRejectAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
