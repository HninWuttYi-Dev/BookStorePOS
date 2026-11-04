using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.PaymentMethod;

public class PaymentMethodPatchRequestModel
{
    public int PaymentMethodId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class PaymentMethodPatchResponseModel : Result<PaymentMethodModel>
{
}
