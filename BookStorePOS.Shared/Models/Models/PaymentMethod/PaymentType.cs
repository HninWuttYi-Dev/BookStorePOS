namespace BookStorePOS.Shared.Models.PaymentMethod;

/// <summary>
/// PaymentType enum with None = 0 as default to avoid accidental fallback to KBZPay.
/// </summary>
public enum PaymentType
{
    None = 0,
    KBZPay = 1,
    WAVEpay = 2,
    WavePay = 2,
    AYAPay = 3,
    AyaPay = 3,
    UabPay = 4,
    CashOnDelivery = 5
}
