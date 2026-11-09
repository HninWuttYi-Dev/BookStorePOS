namespace BookStorePOS.Shared.Models.Order;

public class OrderConfirmRequestModel
{
    public int OrderId { get; set; }
    public string? ConfirmedBy { get; set; }
}
