using BookStorePOS.Domain.Models.Order;

namespace BookStorePOS.Domain.Features.Order;

public interface IOrderService
{
    Task<OrderCreateResponseModel> CreateOrder(OrderCreateRequestModel requestModel);
    Task<OrderGetByIdResponseModel> GetOrder(OrderGetByIdRequestModel requestModel);
    Task<OrderListResponseModel> GetOrdersAsync(OrderListRequestModel requestModel);
    Task<OrderSummaryResponseModel> GetOrderSummaryAsync();
}
