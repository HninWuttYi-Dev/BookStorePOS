using System;
using System.Collections.Generic;

namespace BookStorePOS.ConsoleApp.Models;

public class OrderItemModel
{
    public int OrderItemId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class OrderModel
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemModel> Items { get; set; } = new();
}

public class OrderListResponseModel : BaseResponseModel
{
    public List<OrderModel> Data { get; set; } = new();
}

public class OrderResponseModel : BaseResponseModel
{
    public OrderModel Data { get; set; } = null!;
}

public class CheckoutItemModel
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCreateRequestModel
{
    public List<CheckoutItemModel> Items { get; set; } = new();
}
