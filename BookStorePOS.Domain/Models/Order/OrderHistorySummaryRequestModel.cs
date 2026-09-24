using System;

namespace BookStorePOS.Domain.Models.Order;

public class OrderHistorySummaryRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
