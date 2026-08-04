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
       return Ok(_orderService.GetOrders(requestModel));
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        return Ok(_orderService.GetOrder(new OrderEditRequestModel{OrderId = id}));
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreateRequestModel requestModel)
    {
       return Ok(_orderService.CreateOrder(requestModel));
    }
}
