using System;
using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class OrderHistoryResponseModel : Result<OrderHistoryDataModel>
{
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