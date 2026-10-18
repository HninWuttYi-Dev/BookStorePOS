using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class OrderCreateRequestModel
{
    public List<CheckoutItemModel> Items { get; set; } = new List<CheckoutItemModel>();
    
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public string OrderSource { get; set; } = "POS";
}

public class CheckoutItemModel
{
    public int BookEditionId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCreateResponseModel : Result<OrderModel>
{
}
