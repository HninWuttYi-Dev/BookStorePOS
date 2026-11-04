using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Domain.Features.PaymentMethod;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.PaymentMethod;

namespace BookStorePOS.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodController : BaseController
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly ILogger<PaymentMethodController> _logger;

    public PaymentMethodController(IPaymentMethodService paymentMethodService, ILogger<PaymentMethodController> logger)
    {
        _paymentMethodService = paymentMethodService;
        _logger = logger;
    }

    /// <summary>
    /// Get all payment methods (Admin)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaymentMethodsAsync([FromQuery] PaymentMethodListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("WebApi PaymentMethod => Fetching payment methods");
            var result = await _paymentMethodService.GetPaymentMethodsAsync(requestModel);
            return ExecuteResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebApi PaymentMethod => Exception in GetPaymentMethodsAsync");
            return ExecuteResult(Result<PaymentMethodModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// Get active payment methods for customer checkout
    /// </summary>
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActivePaymentMethodsAsync([FromQuery] PaymentMethodActiveListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("WebApi PaymentMethod => Fetching active payment methods for checkout");
            var result = await _paymentMethodService.GetActivePaymentMethodsAsync(requestModel);
            return ExecuteResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebApi PaymentMethod => Exception in GetActivePaymentMethodsAsync");
            return ExecuteResult(Result<PaymentMethodModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// Get payment method by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var result = await _paymentMethodService.GetPaymentMethodByIdAsync(new PaymentMethodByIdRequestModel { PaymentMethodId = id });
            return ExecuteResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebApi PaymentMethod => Exception in GetByIdAsync");
            return ExecuteResult(Result<PaymentMethodModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// Update payment method account details (Admin)
    /// </summary>
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePaymentMethodAsync(int id, [FromBody] PaymentMethodPatchRequestModel requestModel)
    {
        try
        {
            requestModel.PaymentMethodId = id;
            var result = await _paymentMethodService.UpdatePaymentMethodAsync(requestModel);
            return ExecuteResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebApi PaymentMethod => Exception in UpdatePaymentMethodAsync");
            return ExecuteResult(Result<PaymentMethodModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    /// <summary>
    /// Toggle payment method active/disabled status (Admin)
    /// </summary>
    [HttpPatch("{id}/toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleStatusAsync(int id, [FromBody] PaymentMethodToggleRequestModel requestModel)
    {
        try
        {
            requestModel.PaymentMethodId = id;
            var result = await _paymentMethodService.ToggleStatusAsync(requestModel);
            return ExecuteResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebApi PaymentMethod => Exception in ToggleStatusAsync");
            return ExecuteResult(Result<PaymentMethodModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
