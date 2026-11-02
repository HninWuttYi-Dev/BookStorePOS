namespace BookStorePOS.Shared.Models.Order;

public enum PaymentStatus : byte
{
    Unpaid = 0,
    Submitted = 1,
    Verified = 2,
    Rejected = 3
}
