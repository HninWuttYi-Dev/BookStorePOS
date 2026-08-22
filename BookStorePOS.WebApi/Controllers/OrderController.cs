using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Domain.Models.Order;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderListRequestModel requestModel)
    {
        var response =await _orderService.GetOrdersAsync(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var response =await _orderService.GetOrder(new OrderGetByIdRequestModel{OrderId = id});
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreateRequestModel requestModel)
    {
        var response =await _orderService.CreateOrder(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }
}
