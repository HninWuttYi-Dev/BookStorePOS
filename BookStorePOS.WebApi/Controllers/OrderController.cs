using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Shared.Models.Order;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet("summary")]
    [Permission("Dashboard.View")]
    public async Task<IActionResult> GetOrderSummaryAsync()
    {
        _logger.LogInformation("Get Order Summary Async => Fetching order summary");
        var response = await _orderService.GetOrderSummaryAsync();
        return ExecuteResult(response);
    }

    [HttpGet("history")]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrderHistoryAsync([FromQuery] OrderHistoryRequestModel requestModel)
    {
        _logger.LogInformation("Get Order History Async => Fetching order history");
        var response = await _orderService.GetOrderHistoryAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderListRequestModel requestModel)
    {
        _logger.LogInformation("Get Orders Async => Fetching all orders");
        var response = await _orderService.GetOrdersAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet("{id}")]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        _logger.LogInformation("Get Order Async => Fetching order by Id: {OrderId}", id);
        var response = await _orderService.GetOrderById(new OrderGetByIdRequestModel { OrderId = id });
        return ExecuteResult(response);
    }

    [HttpPost]
    [Permission("Order.Create")]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Order Async => Creating order");
        var response = await _orderService.CreateOrder(requestModel);
        return ExecuteResult(response);
    }
}
