namespace BookStorePOS.Domain.Models.OrderItem;

public class OrderItemDeleteRequestModel
{
    public int OrderItemId { get; set; }
}

public class OrderItemDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderItemModel Data { get; set; } = null!;
}
