using System;
using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.Order;

public class OrderListRequestModel
{
}

public class OrderListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<OrderModel> Data { get; set; } = null!;
}

public class OrderModel
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity {get; set;}
    public OrderStatus OrderStatus { get; set; }
    public string OrderSource { get; set; } = null!;
    public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
}

public class OrderItemModel
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int BookEditionId { get; set; }
    public string? BookTitle { get; set; }
    public string? EditionName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Subtotal { get; set; }
}
