using System;
using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class OrderListRequestModel
{
}

public class OrderListResponseModel : Result<List<OrderModel>>
{
}

public class OrderModel
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string OrderSource { get; set; } = null!;
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentScreenshotUrl { get; set; }
    public string? RejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
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
