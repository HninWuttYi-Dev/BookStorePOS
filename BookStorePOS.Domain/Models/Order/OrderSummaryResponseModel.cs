using System;

namespace BookStorePOS.Domain.Models.Order;

public class OrderSummaryResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderSummaryModel Data { get; set; } = null!;
}

public class OrderSummaryModel
{
    public decimal todayTotal { get; set; }
    public int todayOrderCount { get; set; }
    public decimal thisMonthTotal { get; set; }
    public int thisMonthOrderCount { get; set; }
}
