using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
            _logger.LogInformation("Orders Async => Fetching order history");

            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}";
            if (requestModel.StartDate.HasValue) qs += $"&StartDate={requestModel.StartDate.Value:yyyy-MM-dd}";
            if (requestModel.EndDate.HasValue) qs += $"&EndDate={requestModel.EndDate.Value:yyyy-MM-dd}";

            var httpResponse = await client.GetAsync($"api/order/history{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<OrderHistoryResponseModel>(jsonString);

            if (response != null && response.isSuccess && response.Data != null)
            {
                ViewData["Orders"] = response.Data.Orders;
                ViewData["Summary"] = response.Data.Summary;
                ViewData["TotalPages"] = response.Data.TotalPages;
                ViewData["CurrentPage"] = response.Data.Page;
                ViewData["StartDate"] = requestModel.StartDate?.ToString("yyyy-MM-dd");
                ViewData["EndDate"] = requestModel.EndDate?.ToString("yyyy-MM-dd");
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

        [ActionName("OrderDetail")]
        public async Task<IActionResult> OrderDetailAsync(int id)
        {
            _logger.LogInformation($"Order Detail Async => Fetching order details for OrderId: {id}");

            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.GetAsync($"api/order/{id}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<OrderGetByIdResponseModel>(jsonString);

            if (response != null && response.isSuccess && response.Data != null)
            {
                _logger.LogInformation($"Order Detail Async => Order details fetched successfully for OrderId: {id}");
                ViewData["OrderDetail"] = response.Data;
                return View("OrderDetail");
            }
            else
            {
                _logger.LogWarning($"Order Detail Async => Failed to fetch order details for OrderId: {id}. Message: {response?.Message}");
                TempData["Message"] = response?.Message;
                TempData["isSuccess"] = false;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> OrderCreateAsync([FromBody] OrderCreateRequestModel requestModel)
        {
            _logger.LogInformation("Order Create Async => Creating new order");

            if (requestModel == null || requestModel.Items == null || requestModel.Items.Count == 0)
            {
                _logger.LogWarning("Order Create Async => No items provided");
                return Json(new OrderCreateResponseModel { isSuccess = false, Message = "Please provide at least one item." });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            var content = new StringContent(JsonConvert.SerializeObject(requestModel), System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync("api/order", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<OrderCreateResponseModel>(jsonString) ?? new OrderCreateResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (response != null && response.isSuccess)
            {
                _logger.LogInformation("Order Create Async => Order created successfully");
            }
            else
            {
                _logger.LogWarning($"Order Create Async => Failed to create order: {response?.Message}");
            }

            return Json(response);
        }
    }
}
