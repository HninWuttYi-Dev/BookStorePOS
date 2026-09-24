using System;

namespace BookStorePOS.Domain.Models.Order;

public class OrderHistoryRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
