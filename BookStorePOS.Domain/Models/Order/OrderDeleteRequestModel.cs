namespace BookStorePOS.Domain.Models.Order;

public class OrderDeleteRequestModel
{
    public int OrderId { get; set; }
}

public class OrderDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderModel Data { get; set; } = null!;
}
