using System;
using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.Order;

public class OrderListRequestModel
{
    public int? UserId { get; set; }
}

public class OrderListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<OrderModel> Data { get; set; } = null!;
}

public class OrderModel
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
}
