using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.PaymentMethod;

public class PaymentMethodByIdRequestModel
{
    public int PaymentMethodId { get; set; }
}

public class PaymentMethodByIdResponseModel : Result<PaymentMethodModel>
{
}

