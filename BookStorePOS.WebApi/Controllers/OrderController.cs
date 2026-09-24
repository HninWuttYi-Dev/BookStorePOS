using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Domain.Models.Order;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetOrderSummaryAsync()
    {
        _logger.LogInformation("Get Order Summary Async => Fetching order summary");
        var response = await _orderService.GetOrderSummaryAsync();
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Order Summary Async => Failed to fetch order summary {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Order Summary Async => Order summary fetched successfully");
        return Ok(response);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetOrderHistoryAsync([FromQuery] OrderHistoryRequestModel requestModel)
    {
        _logger.LogInformation("Get Order History Async => Fetching order history");
        var response = await _orderService.GetOrderHistoryAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Order History Async => Failed to fetch order history {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Order History Async => Order history fetched successfully");
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderListRequestModel requestModel)
    {
        _logger.LogInformation("Get Orders Async => Fetching all orders");
        var response =await _orderService.GetOrdersAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Orders Async => Failed to fetch orders {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Orders Async => Orders fetched successfully");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        _logger.LogInformation("Get Order Async => Fetching order by Id");
        var response =await _orderService.GetOrderById(new OrderGetByIdRequestModel{OrderId = id});
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Order Async => Failed to fetch order {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Order Async => Order fetched successfully");
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Order Async => Creating order");
        var response =await _orderService.CreateOrder(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Create Order Async => Failed to create order {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Create Order Async => Order is created successfully");
        return Ok(response);
    }
}
