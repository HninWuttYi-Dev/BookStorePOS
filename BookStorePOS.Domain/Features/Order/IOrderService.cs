using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;

namespace BookStorePOS.Domain.Features.Order;

public interface IOrderService
{
    Task<Result<OrderModel>> CreateOrder(OrderCreateRequestModel requestModel);
    Task<Result<OrderModel>> GetOrderById(OrderGetByIdRequestModel requestModel);
    Task<OrderListResponseModel> GetOrdersAsync(OrderListRequestModel requestModel);
    Task<OrderSummaryResponseModel> GetOrderSummaryAsync();
    Task<OrderHistoryResponseModel> GetOrderHistoryAsync(OrderHistoryRequestModel requestModel);
}

