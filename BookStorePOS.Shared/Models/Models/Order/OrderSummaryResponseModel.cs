using System;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class OrderSummaryResponseModel : Result<OrderSummaryModel>
{
}

public class OrderSummaryModel
{
    public decimal todayTotalRevenue { get; set; }
    public int todayOrderCount { get; set; }
    public decimal thisMonthTotalRevenue { get; set; }
    public int thisMonthOrderCount { get; set; }
    public int pendingOnlineOrderCount { get; set; }
    public decimal todayGrowthPercentage { get; set; }
    public decimal monthGrowthPercentage { get; set; }
}
