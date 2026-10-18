using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class OrderGetByIdRequestModel
{
    public int OrderId { get; set; }
}

public class OrderGetByIdResponseModel : Result<OrderModel>
{
}
