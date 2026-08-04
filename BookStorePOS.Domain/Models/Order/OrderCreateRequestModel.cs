namespace BookStorePOS.Domain.Models.Order;

public class OrderCreateRequestModel
{
    public int UserId { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderModel Data { get; set; } = null!;
}
