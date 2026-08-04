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
    public OrderController()
    {
        _orderService = new OrderService();
    }

    [HttpGet]
    public IActionResult GetOrders([FromQuery] OrderListRequestModel requestModel)
    {
        var response = _orderService.GetOrders(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        var response = _orderService.GetOrder(new OrderEditRequestModel{OrderId = id});
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreateRequestModel requestModel)
    {
        var response = _orderService.CreateOrder(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }
}
