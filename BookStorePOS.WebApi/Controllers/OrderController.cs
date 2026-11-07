using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Notification;
using BookStorePOS.Shared.Models.Order;
using BookStorePOS.WebApi.Filters;
using BookStorePOS.WebApi.Hubs;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService, 
        IFileStorageService fileStorageService,
        IHubContext<NotificationHub> hubContext,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _fileStorageService = fileStorageService;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpGet("summary")]
    [Permission("Dashboard.View")]
    public async Task<IActionResult> GetOrderSummaryAsync()
    {
        _logger.LogInformation("Get Order Summary Async => Fetching order summary");
        var response = await _orderService.GetOrderSummaryAsync();
        return ExecuteResult(response);
    }

    [HttpGet("revenue-chart")]
    [Permission("Dashboard.View")]
    public async Task<IActionResult> GetSalesRevenueChartAsync([FromQuery] SalesRevenueChartRequestModel requestModel)
    {
        _logger.LogInformation("Get Sales Revenue Chart Async => Fetching chart data");
        var response = await _orderService.GetSalesRevenueChartAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet("top-sellers")]
    [Permission("Dashboard.View")]
    public async Task<IActionResult> GetTopSellersAsync([FromQuery] TopSellersRequestModel requestModel)
    {
        _logger.LogInformation("Get Top Sellers Async => Fetching top sellers");
        var response = await _orderService.GetTopSellersAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet("top-sellers-dashboard")]
    [Permission("Dashboard.View")]
    public async Task<IActionResult> GetDashboardTopSellersAsync([FromQuery] int limit = 5)
    {
        _logger.LogInformation("Get Dashboard Top Sellers Async => Fetching dashboard top sellers");
        var response = await _orderService.GetDashboardTopSellersAsync(limit);
        return ExecuteResult(response);
    }

    [HttpGet("history")]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrderHistoryAsync([FromQuery] OrderHistoryRequestModel requestModel)
    {
        _logger.LogInformation("Get Order History Async => Fetching order history");
        var response = await _orderService.GetOrderHistoryAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderListRequestModel requestModel)
    {
        _logger.LogInformation("Get Orders Async => Fetching all orders");

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int uid) && uid > 0)
            {
                requestModel.CustomerId = uid;
            }
        }

        var response = await _orderService.GetOrdersAsync(requestModel);
        return ExecuteResult(response);
    }

    /// <summary>
    /// GET api/order/{id}
    /// Can be accessed by staff, cashiers (for checkout voucher), or customers
    /// </summary>
    [HttpGet("{id}")]
    [Permission("Order.View,Order.Create")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        _logger.LogInformation("Get Order Async => Fetching order by Id: {OrderId}", id);
        var response = await _orderService.GetOrderById(new OrderGetByIdRequestModel { OrderId = id });

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase) && response.IsSuccess && response.Data != null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int uid) && response.Data.CustomerId != uid)
            {
                return ExecuteResult(Result<OrderModel>.Unauthorized("You are not authorized to view this order."));
            }
        }

        return ExecuteResult(response);
    }

    /// <summary>
    /// Existing POS create order: OrderSource POS, OrderStatus Completed.
    /// </summary>
    [HttpPost]
    [Permission("Order.Create")]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Order Async => Creating POS order");
        var response = await _orderService.CreateOrder(requestModel);
        if (response.IsSuccess && response.Data != null)
        {
            await BroadcastDashboardUpdateAsync(response.Data.OrderId);
        }
        return ExecuteResult(response);
    }

    /// <summary>
    /// POST api/order/online
    /// Online customer order: OrderSource Online, OrderStatus Pending, PaymentStatus Submitted/Unpaid.
    /// Supports multipart/form-data with payment screenshot file or JSON body.
    /// </summary>
    [HttpPost("online")]
    [Permission("Order.Create")]
    public async Task<IActionResult> CreateOnlineOrderAsync()
    {
        try
        {
            OnlineOrderCreateRequestModel? requestModel = null;
            IFormFile? screenshotFile = null;

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                
                string? itemsJson = form["items"].FirstOrDefault() ?? form["itemsJson"].FirstOrDefault();
                List<OnlineOrderItemRequestModel> itemsList = new();
                if (!string.IsNullOrWhiteSpace(itemsJson))
                {
                    try
                    {
                        itemsList = JsonSerializer.Deserialize<List<OnlineOrderItemRequestModel>>(itemsJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize itemsJson: {Json}", itemsJson);
                    }
                }

                requestModel = new OnlineOrderCreateRequestModel
                {
                    CustomerId = int.TryParse(form["customerId"].FirstOrDefault(), out var cId) ? cId : null,
                    CustomerName = form["customerName"].FirstOrDefault(),
                    CustomerPhone = form["customerPhone"].FirstOrDefault(),
                    ShippingAddress = form["shippingAddress"].FirstOrDefault(),
                    Notes = form["notes"].FirstOrDefault(),
                    PaymentMethod = form["paymentMethod"].FirstOrDefault(),
                    PaymentScreenshotUrl = form["paymentScreenshotUrl"].FirstOrDefault(),
                    Items = itemsList
                };

                if (form.Files.Count > 0 && form.Files[0].Length > 0)
                {
                    screenshotFile = form.Files[0];
                }
            }
            else
            {
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(rawBody))
                {
                    requestModel = JsonSerializer.Deserialize<OnlineOrderCreateRequestModel>(rawBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            if (requestModel == null)
            {
                return ExecuteResult(Result<OrderModel>.ValidationError("Invalid request model."));
            }

            // If CustomerId not provided in form, extract from JWT claims
            if (!requestModel.CustomerId.HasValue || requestModel.CustomerId.Value <= 0)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int uid) && uid > 0)
                {
                    requestModel.CustomerId = uid;
                }
            }

            // Upload screenshot file if attached
            if (screenshotFile != null && screenshotFile.Length > 0)
            {
                var uploadedUrl = await _fileStorageService.UploadImageAsync(screenshotFile, "payments");
                if (!string.IsNullOrWhiteSpace(uploadedUrl))
                {
                    requestModel.PaymentScreenshotUrl = uploadedUrl;
                }
            }

            _logger.LogInformation("Create Online Order Async => Creating online order for customer {CustomerName}", requestModel.CustomerName);
            var response = await _orderService.CreateOnlineOrderAsync(requestModel);

            if (response.IsSuccess && response.Data != null)
            {
                // Only broadcast notification for fresh new orders (suppress duplicate broadcast on network retries)
                bool isDuplicateRetry = string.Equals(response.Message, "Order already received.", StringComparison.OrdinalIgnoreCase);
                if (!isDuplicateRetry)
                {
                    var order = response.Data;
                    var orderCode = $"C{(order.OrderDate ?? DateTime.Now):yyMMdd}{order.OrderId:0000}";

                    // 1. Alert Staff of incoming new online order
                    var notifStaff = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = "New Online Order",
                        Message = $"Order #{orderCode} placed by {order.CustomerName ?? "Customer"} ({order.TotalPrice:N0} MMK)",
                        Type = "OrderPlaced",
                        OrderId = order.OrderId,
                        OrderCode = orderCode,
                        TargetUrl = $"/OnlineOrders?orderId={order.OrderId}&orderNo={orderCode}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _hubContext.Clients.Group("Staff").SendAsync("ReceiveNotification", notifStaff);

                    // 2. Alert the specific Customer that their order was submitted successfully
                    if (order.CustomerId.HasValue && order.CustomerId.Value > 0)
                    {
                        var notifCust = new NotificationModel
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Title = "Order Placed",
                            Message = $"Your order #{orderCode} has been submitted successfully and is pending confirmation.",
                            Type = "OrderPlaced",
                            OrderId = order.OrderId,
                            OrderCode = orderCode,
                            TargetUrl = $"/Orders/Detail/{order.OrderId}",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _hubContext.Clients.Group($"User_{order.CustomerId.Value}").SendAsync("ReceiveNotification", notifCust);
                    }
                }

                await BroadcastDashboardUpdateAsync(response.Data.OrderId);
            }

            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Online Order Async => Exception occurred");
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// 3) POST api/order/{id}/confirm
    /// Admin or authorized Staff confirms pending order: checks stock again, deducts stock, sets Completed/Verified.
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Permission("Order.Confirm")]
    public async Task<IActionResult> ConfirmOrderAsync(int id)
    {
        try
        {
            _logger.LogInformation("Confirm Order Async => Confirming order {OrderId}", id);
            var response = await _orderService.ConfirmOrderAsync(new OrderConfirmRequestModel { OrderId = id });

            if (response.IsSuccess && response.Data != null)
            {
                var order = response.Data;
                var orderCode = $"C{(order.OrderDate ?? DateTime.Now):yyMMdd}{order.OrderId:0000}";
                
                // Notify Customer ONLY (isolated by CustomerId to the user who placed this order)
                if (order.CustomerId.HasValue && order.CustomerId.Value > 0)
                {
                    var notifCust = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = "Order Confirmed!",
                        Message = $"Your order #{orderCode} has been verified and confirmed.",
                        Type = "OrderConfirmed",
                        OrderId = order.OrderId,
                        OrderCode = orderCode,
                        TargetUrl = $"/Orders/Detail/{order.OrderId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _hubContext.Clients.Group($"User_{order.CustomerId.Value}").SendAsync("ReceiveNotification", notifCust);
                }

                await BroadcastDashboardUpdateAsync(response.Data.OrderId);
            }

            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirm Order Async => Exception occurred for order {OrderId}", id);
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// 4) POST api/order/{id}/reject
    /// Admin or authorized Staff rejects pending order: requires RejectReason, sets Rejected/Rejected, no stock change.
    /// </summary>
    [HttpPost("{id}/reject")]
    [Permission("Order.Reject")]
    public async Task<IActionResult> RejectOrderAsync(int id, [FromBody] OrderRejectRequestModel? requestModel)
    {
        try
        {
            _logger.LogInformation("Reject Order Async => Rejecting order {OrderId}", id);
            requestModel ??= new OrderRejectRequestModel();
            requestModel.OrderId = id;
            var response = await _orderService.RejectOrderAsync(requestModel);

            if (response.IsSuccess && response.Data != null)
            {
                var order = response.Data;
                var orderCode = $"C{(order.OrderDate ?? DateTime.Now):yyMMdd}{order.OrderId:0000}";
                
                // Notify Customer ONLY (isolated by CustomerId to the user who placed this order)
                if (order.CustomerId.HasValue && order.CustomerId.Value > 0)
                {
                    var notifCust = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = "Order Rejected",
                        Message = $"Your order #{orderCode} was rejected. Reason: {requestModel.RejectReason ?? "Details invalid"}",
                        Type = "OrderRejected",
                        OrderId = order.OrderId,
                        OrderCode = orderCode,
                        TargetUrl = $"/Orders/Detail/{order.OrderId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _hubContext.Clients.Group($"User_{order.CustomerId.Value}").SendAsync("ReceiveNotification", notifCust);
                }

                await BroadcastDashboardUpdateAsync(response.Data.OrderId);
            }

            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reject Order Async => Exception occurred for order {OrderId}", id);
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    private async Task BroadcastDashboardUpdateAsync(int? latestOrderId = null)
    {
        try
        {
            var summaryResp = await _orderService.GetDashboardRealtimeSummaryAsync(latestOrderId);
            if (summaryResp.IsSuccess && summaryResp.Data != null)
            {
                await _hubContext.Clients.Group("Staff").SendAsync("ReceiveDashboardUpdateEvent", summaryResp.Data);
                _logger.LogInformation("BroadcastDashboardUpdateAsync => Real-time dashboard update dispatched to Staff group");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BroadcastDashboardUpdateAsync => Failed to broadcast real-time dashboard update");
        }
    }
}
