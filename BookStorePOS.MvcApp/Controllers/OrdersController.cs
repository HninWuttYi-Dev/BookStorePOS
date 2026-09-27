using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BookStorePOS.MvcApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> OrdersAsync(OrderHistoryRequestModel requestModel)
        {
            _logger.LogInformation("Orders Async => Fetching order history");

            var response = await _orderService.GetOrderHistoryAsync(requestModel);

            if (response.isSuccess && response.Data != null)
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
                _logger.LogWarning($"Orders Async => Failed to fetch order history: {response.Message}");
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
            }

            return View("Order");
        }

        [ActionName("OrderDetail")]
        public async Task<IActionResult> OrderDetailAsync(int id)
        {
            _logger.LogInformation($"Order Detail Async => Fetching order details for OrderId: {id}");

            var response = await _orderService.GetOrderById(new OrderGetByIdRequestModel { OrderId = id });

            if (response.isSuccess && response.Data != null)
            {
                _logger.LogInformation($"Order Detail Async => Order details fetched successfully for OrderId: {id}");
                ViewData["OrderDetail"] = response.Data;
                return View("OrderDetail");
            }
            else
            {
                _logger.LogWarning($"Order Detail Async => Failed to fetch order details for OrderId: {id}. Message: {response.Message}");
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
                return RedirectToAction("Index");
            }
        }
    }
}
