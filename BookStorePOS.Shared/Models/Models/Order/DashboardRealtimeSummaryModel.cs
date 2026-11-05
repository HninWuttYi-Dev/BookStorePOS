using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class DashboardRealtimeResponseModel : Result<DashboardRealtimeSummaryModel>
{
}

public class DashboardRealtimeSummaryModel
{
    // Metric cards
    public decimal TodayRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public int LowStockItemsCount { get; set; }
    public int PendingOnlineOrdersCount { get; set; }
    public decimal TodayGrowthPercentage { get; set; }
    public decimal MonthGrowthPercentage { get; set; }

    // Sales Revenue Chart
    public SalesRevenueChartDataModel? RevenueChart { get; set; }

    // Top Sellers Bar Chart
    public DashboardTopSellersDataModel? TopSellers { get; set; }

    // Latest Order for Recent Orders table
    public RecentOrderItemModel? LatestOrder { get; set; }
}

public class RecentOrderItemModel
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string DateFormatted { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBgColor { get; set; } = "#E7F3EA";
    public string StatusTextColor { get; set; } = "#15803D";
    public string StatusDotColor { get; set; } = "#15803D";
}
