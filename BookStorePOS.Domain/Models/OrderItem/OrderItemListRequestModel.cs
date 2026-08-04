using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.OrderItem;

public class OrderItemListRequestModel
{
    public int OrderId { get; set; }
}

public class OrderItemListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<OrderItemModel> Data { get; set; } = null!;
}

public class OrderItemModel
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Subtotal { get; set; }
}
