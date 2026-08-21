namespace BookStorePOS.Domain.Models.Order;

public class OrderGetByIdRequestModel
{
    public int OrderId { get; set; }
}

public class OrderGetByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderModel Data { get; set; } = null!;
}
