namespace BookStorePOS.Shared.Models.Order;

public class OrderRejectRequestModel
{
    public int OrderId { get; set; }
    public string? RejectReason { get; set; }
}
