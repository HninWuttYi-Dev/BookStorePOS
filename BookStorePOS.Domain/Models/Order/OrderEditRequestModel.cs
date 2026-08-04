namespace BookStorePOS.Domain.Models.Order;

public class OrderEditRequestModel
{
    public int OrderId { get; set; }
}

public class OrderEditResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderModel Data { get; set; } = null!;
}
