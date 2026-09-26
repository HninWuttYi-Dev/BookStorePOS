using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BookStorePOS.MvcApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> OrdersAsync(OrderHistoryRequestModel requestModel)
        {

            var response = await _orderService.GetOrderHistoryAsync(requestModel);

            if (response.isSuccess && response.Data != null)
            {
                ViewData["Orders"] = response.Data.Orders;
                ViewData["Summary"] = response.Data.Summary;
                ViewData["TotalPages"] = response.Data.TotalPages;
                ViewData["CurrentPage"] = response.Data.Page;
                ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
                ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            }
            else
            {
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
            }

            return View("Order");
        }
    }
}
