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
        public async Task<IActionResult> OnlineOrderListAsync(string? tab = "Pending", string? search = "")
        {
            try
            {
                _logger.LogInformation("Online Order List Async => Fetching online orders");

                tab = string.IsNullOrWhiteSpace(tab) ? "Pending" : tab;
                var qs = "?OrderSource=Online";
                if (tab == "Pending") qs += "&OrderStatus=Pending";
                else if (tab == "Completed") qs += "&OrderStatus=Completed";
                else if (tab == "Rejected") qs += "&OrderStatus=Rejected";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    qs += $"&Search={Uri.EscapeDataString(search.Trim())}";
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/order{qs}");
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
                }

                return View("OnlineOrderList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnlineOrderListAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
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
