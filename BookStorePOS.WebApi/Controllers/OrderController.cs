using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService, 
        IFileStorageService fileStorageService,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _fileStorageService = fileStorageService;
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
        var response = await _orderService.GetOrdersAsync(requestModel);
        return ExecuteResult(response);
    }

    [HttpGet("{id}")]
    [Permission("Order.View")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        _logger.LogInformation("Get Order Async => Fetching order by Id: {OrderId}", id);
        var response = await _orderService.GetOrderById(new OrderGetByIdRequestModel { OrderId = id });
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
        return ExecuteResult(response);
    }

    /// <summary>
    /// 1) POST api/order/online
    /// Online customer order: OrderSource Online, OrderStatus Pending, PaymentStatus Unpaid.
    /// </summary>
    [HttpPost("online")]
    [Permission("Order.Create")]
    public async Task<IActionResult> CreateOnlineOrderAsync([FromBody] OnlineOrderCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create Online Order Async => Creating online order for customer {CustomerName}", requestModel.CustomerName);
            var response = await _orderService.CreateOnlineOrderAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Online Order Async => Exception occurred");
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// 2) POST api/order/{id}/payment-screenshot
    /// Upload payment screenshot via file upload (multipart/form-data) or URL (application/json or form).
    /// </summary>
    [HttpPost("{id}/payment-screenshot")]
    [Permission("Order.Create")]
    public async Task<IActionResult> UploadPaymentScreenshotAsync(int id)
    {
        try
        {
            _logger.LogInformation("Upload Payment Screenshot Async => Uploading payment screenshot for order {OrderId}", id);
            string? screenshotUrl = null;

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.Files.Count > 0 && form.Files[0].Length > 0)
                {
                    screenshotUrl = await _fileStorageService.UploadImageAsync(form.Files[0], "payments");
                }
                if (string.IsNullOrWhiteSpace(screenshotUrl) && form.TryGetValue("paymentScreenshotUrl", out var urlVal))
                {
                    screenshotUrl = urlVal.ToString();
                }
            }
            else
            {
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(rawBody))
                {
                    var model = JsonSerializer.Deserialize<OrderPaymentScreenshotRequestModel>(rawBody, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    screenshotUrl = model?.PaymentScreenshotUrl;
                }
            }

            if (string.IsNullOrWhiteSpace(screenshotUrl))
            {
                return ExecuteResult(Result<OrderModel>.ValidationError("Payment screenshot URL or file is required."));
            }

            var req = new OrderPaymentScreenshotRequestModel
            {
                OrderId = id,
                PaymentScreenshotUrl = screenshotUrl
            };

            var response = await _orderService.UploadPaymentScreenshotAsync(req);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload Payment Screenshot Async => Exception occurred for order {OrderId}", id);
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// 3) POST api/order/{id}/confirm
    /// Admin confirms pending order: checks stock again, deducts stock, sets Completed/Verified.
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Permission("Order.Create")]
    public async Task<IActionResult> ConfirmOrderAsync(int id)
    {
        try
        {
            _logger.LogInformation("Confirm Order Async => Confirming order {OrderId}", id);
            var response = await _orderService.ConfirmOrderAsync(new OrderConfirmRequestModel { OrderId = id });
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
    /// Admin rejects pending order: requires RejectReason, sets Rejected/Rejected, no stock change.
    /// </summary>
    [HttpPost("{id}/reject")]
    [Permission("Order.Create")]
    public async Task<IActionResult> RejectOrderAsync(int id, [FromBody] OrderRejectRequestModel? requestModel)
    {
        try
        {
            _logger.LogInformation("Reject Order Async => Rejecting order {OrderId}", id);
            requestModel ??= new OrderRejectRequestModel();
            requestModel.OrderId = id;
            var response = await _orderService.RejectOrderAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reject Order Async => Exception occurred for order {OrderId}", id);
            return ExecuteResult(Result<OrderModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
