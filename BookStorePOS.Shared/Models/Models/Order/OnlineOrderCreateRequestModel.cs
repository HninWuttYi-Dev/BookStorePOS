using System;
using System.Collections.Generic;
using BookStorePOS.Shared.Models.PaymentMethod;

namespace BookStorePOS.Shared.Models.Order;

public class OnlineOrderCreateRequestModel
{
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }
    public PaymentType PaymentType
    {
        get
        {
            if (Enum.TryParse<PaymentType>(PaymentMethod, true, out var type))
            {
                return type;
            }
            return PaymentType.None;
        }
        set => PaymentMethod = value != PaymentType.None ? value.ToString() : null;
    }
    public string? PaymentScreenshotUrl { get; set; }
    public List<OnlineOrderItemRequestModel> Items { get; set; } = new List<OnlineOrderItemRequestModel>();
}

public class OnlineOrderItemRequestModel
{
    public int BookEditionId { get; set; }
    public int Quantity { get; set; }
}
