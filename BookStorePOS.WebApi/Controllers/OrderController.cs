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

    /// <summary>
    /// GET api/order/{id}
    /// Can be accessed by staff or customers with Order.View permission
    /// </summary>
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
