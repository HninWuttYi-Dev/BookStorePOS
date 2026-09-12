using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Domain.Models.Order;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BookStorePOS.Chart.Controllers;

public class ChartjsController : Controller
{
    private readonly OrderService _orderService;

    public ChartjsController(OrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var orderResponse = await _orderService.GetOrdersAsync(new OrderListRequestModel());
        var orders = orderResponse.isSuccess ? orderResponse.Data : new List<OrderModel>();

        var groupedOrders = orders
            .Where(o => o.OrderDate.HasValue)
            .GroupBy(o => o.OrderDate.Value.Date)
            .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Total = g.Sum(o => o.TotalPrice) })
            .OrderBy(x => x.Date)
            .ToList();
            
        List<decimal> series = groupedOrders.Select(x => x.Total).ToList();
        List<string> labels = groupedOrders.Select(x => x.Date).ToList();

        ViewData["Series"] = series;
        ViewData["Labels"] = labels;

        return View();
    }
}
