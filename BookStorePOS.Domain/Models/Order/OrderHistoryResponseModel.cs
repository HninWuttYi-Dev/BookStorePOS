using System;
using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.Order;

public class OrderHistoryResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public OrderHistoryDataModel Data { get; set; } = null!;
}

public class OrderHistoryDataModel
{
    public OrderHistorySummaryModel Summary { get; set; } = null!;
    public List<OrderModel> Orders { get; set; } = new List<OrderModel>();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages {get; set;}
}
public class OrderHistorySummaryModel
{
    public int TotalOrders {get; set;}
    public decimal TotalRevenue {get; set;}
    public decimal AverageOrderValue {get;set;}
}