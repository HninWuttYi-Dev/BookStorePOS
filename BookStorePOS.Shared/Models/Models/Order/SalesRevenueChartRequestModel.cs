using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class SalesRevenueChartRequestModel
{
    public string? Period { get; set; } // "7days", "30days", "month"
}

public class SalesRevenueChartResponseModel : Result<SalesRevenueChartDataModel>
{
}

public class SalesRevenueChartDataModel
{
    public ChartPeriodData SevenDays { get; set; } = new();
    public ChartPeriodData ThirtyDays { get; set; } = new();
    public ChartPeriodData ThisMonth { get; set; } = new();
}

public class ChartPeriodData
{
    public List<string> Categories { get; set; } = new();
    public List<decimal> Series { get; set; } = new(); // Overall / Total
    public List<decimal> PosSeries { get; set; } = new(); // POS in-store sales
    public List<decimal> OnlineSeries { get; set; } = new(); // Online orders sales
    public string PeakValueFormatted { get; set; } = string.Empty;
    public int PeakIndex { get; set; }
}

