using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.PaymentMethod;

public class PaymentMethodToggleRequestModel
{
    public int PaymentMethodId { get; set; }
    public bool IsActive { get; set; }
}

public class PaymentMethodToggleResponseModel : Result<PaymentMethodModel>
{
}

