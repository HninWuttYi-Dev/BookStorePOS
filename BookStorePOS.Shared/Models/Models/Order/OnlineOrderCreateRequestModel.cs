using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.Order;

public class OnlineOrderCreateRequestModel
{
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public List<OnlineOrderItemRequestModel> Items { get; set; } = new List<OnlineOrderItemRequestModel>();
}

public class OnlineOrderItemRequestModel
{
    public int BookEditionId { get; set; }
    public int Quantity { get; set; }
}
