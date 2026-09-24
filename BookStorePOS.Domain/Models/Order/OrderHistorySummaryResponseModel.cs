using System;

namespace BookStorePOS.Domain.Models.Order;

public class OrderHistorySummaryResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderHistorySummaryModel Data { get; set; } = null!;
}

public class OrderHistorySummaryModel
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}
