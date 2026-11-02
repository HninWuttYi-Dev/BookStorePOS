namespace BookStorePOS.Shared.Models.Order;

public class OrderPaymentScreenshotRequestModel
{
    public int OrderId { get; set; }
    public string? PaymentScreenshotUrl { get; set; }
}
