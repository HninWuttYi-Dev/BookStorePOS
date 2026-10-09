using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblOrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Subtotal { get; set; }

    public int? BookEditionId { get; set; }

    public virtual TblBookEdition? BookEdition { get; set; }

    public virtual TblOrder Order { get; set; } = null!;
}
