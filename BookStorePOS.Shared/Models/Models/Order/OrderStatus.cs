namespace BookStorePOS.Shared.Models.Order;

public enum OrderStatus : byte
{
    None = 0,
    Pending = 1,
    Completed = 2,
    Rejected = 3,
    Cancelled = 4
}
