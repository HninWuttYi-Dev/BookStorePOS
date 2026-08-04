using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.OrderItem;
using BookStorePOS.Domain.Models.OrderItem;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemController : ControllerBase
{
    private readonly OrderItemService _orderItemService;
    public OrderItemController()
    {
        _orderItemService = new OrderItemService();
    }

    [HttpGet]
    public IActionResult GetOrderItems([FromQuery] OrderItemListRequestModel requestModel)
    {
       return Ok(_orderItemService.GetOrderItemsByOrderId(requestModel));
    }

    [HttpPost]
    public IActionResult AddOrderItem([FromBody] OrderItemCreateRequestModel requestModel)
    {
       return Ok(_orderItemService.AddOrderItem(requestModel));
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveOrderItem(int id)
    {
        return Ok(_orderItemService.RemoveOrderItem(new OrderItemDeleteRequestModel{OrderItemId = id}));
    }
}
