using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.PaymentMethod;

public class PaymentMethodActiveListRequestModel
{
}

public class PaymentMethodActiveListResponseModel : Result<List<PaymentMethodModel>>
{
}

