using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.Order;

public class OrderCreateRequestModel
{
    public int UserId { get; set; }
    public List<CheckoutItemModel> Items { get; set; } = new List<CheckoutItemModel>();
}

public class CheckoutItemModel
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderModel Data { get; set; } = null!;
}
