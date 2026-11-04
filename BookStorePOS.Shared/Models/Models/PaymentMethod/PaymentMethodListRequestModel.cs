using System;
using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.PaymentMethod;

public class PaymentMethodListRequestModel
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}

public class PaymentMethodListResponseModel : Result<List<PaymentMethodModel>>
{
    public int Count { get; set; }
}

public class PaymentMethodModel
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
    public PaymentType PaymentType
    {
        get
        {
            if (Enum.TryParse<PaymentType>(PaymentCode, true, out var type))
            {
                return type;
            }
            return PaymentType.None;
        }
    }
    public string LogoUrl => (PaymentCode?.Trim()) switch
    {
        "KBZPay" => "/images/payments/kpay.png",
        "WavePay" or "WAVEpay" => "/images/payments/wavepay.png",
        "AyaPay" or "AYAPay" => "/images/payments/ayapay.png",
        "UabPay" or "uabPay" => "/images/payments/uabpay.png",
        "CashOnDelivery" or "COD" => "/images/payments/cod.png",
        _ => "/images/payments/cod.png"
    };
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

