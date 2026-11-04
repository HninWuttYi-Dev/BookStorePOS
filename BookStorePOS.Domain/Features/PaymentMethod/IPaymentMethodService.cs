using System.Threading.Tasks;
using BookStorePOS.Shared.Models.PaymentMethod;

namespace BookStorePOS.Domain.Features.PaymentMethod;

public interface IPaymentMethodService
{
    Task<PaymentMethodListResponseModel> GetPaymentMethodsAsync(PaymentMethodListRequestModel requestModel);
    Task<PaymentMethodActiveListResponseModel> GetActivePaymentMethodsAsync(PaymentMethodActiveListRequestModel requestModel);
    Task<PaymentMethodByIdResponseModel> GetPaymentMethodByIdAsync(PaymentMethodByIdRequestModel requestModel);
    Task<PaymentMethodPatchResponseModel> UpdatePaymentMethodAsync(PaymentMethodPatchRequestModel requestModel);
    Task<PaymentMethodToggleResponseModel> ToggleStatusAsync(PaymentMethodToggleRequestModel requestModel);
}
