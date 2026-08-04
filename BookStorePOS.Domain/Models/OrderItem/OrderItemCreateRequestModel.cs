namespace BookStorePOS.Domain.Models.OrderItem;

public class OrderItemCreateRequestModel
{
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderItemCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderItemModel Data { get; set; } = null!;
}
