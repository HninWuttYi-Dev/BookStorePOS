using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblPaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string PaymentCode { get; set; } = null!;

    public string PaymentName { get; set; } = null!;

    public string? AccountName { get; set; }

    public string? AccountNumber { get; set; }

    public string? BadgeCode { get; set; }

    public string? BadgeBg { get; set; }

    public string? Description { get; set; }

    public bool IsSlipRequired { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
