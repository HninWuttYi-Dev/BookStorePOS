using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblOrder
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public int TotalQuantity { get; set; }

    public byte OrderStatus { get; set; }

    public string OrderSource { get; set; } = null!;

    public int? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TblUser? Customer { get; set; }

    public virtual ICollection<TblOrderItem> TblOrderItems { get; set; } = new List<TblOrderItem>();
}
