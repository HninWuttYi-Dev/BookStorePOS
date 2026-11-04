using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.PaymentMethod;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.PaymentMethod;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly AppDbContext _db;
    private readonly ILogger<PaymentMethodService> _logger;

    public PaymentMethodService(AppDbContext db, ILogger<PaymentMethodService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PaymentMethodListResponseModel> GetPaymentMethodsAsync(PaymentMethodListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("PaymentMethodService => Fetching payment methods");
            var query = _db.TblPaymentMethods.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(requestModel.Search))
            {
                var s = requestModel.Search.Trim();
                query = query.Where(p => p.PaymentName.Contains(s) || p.PaymentCode.Contains(s));
            }

            if (requestModel.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == requestModel.IsActive.Value);
            }

            var list = await query
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.PaymentMethodId)
                .Select(p => MapToModel(p))
                .ToListAsync();

            return new PaymentMethodListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Payment methods fetched successfully.",
                Data = list,
                Count = list.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentMethodService => Failed to fetch payment methods");
            return new PaymentMethodListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch payment methods: " + ex.Message
            };
        }
    }

    public async Task<PaymentMethodActiveListResponseModel> GetActivePaymentMethodsAsync(PaymentMethodActiveListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("PaymentMethodService => Fetching active payment methods");
            var list = await _db.TblPaymentMethods
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.PaymentMethodId)
                .Select(p => MapToModel(p))
                .ToListAsync();

            return new PaymentMethodActiveListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Active payment methods fetched successfully.",
                Data = list
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentMethodService => Failed to fetch active payment methods");
            return new PaymentMethodActiveListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch active payment methods: " + ex.Message
            };
        }
    }

    public async Task<PaymentMethodByIdResponseModel> GetPaymentMethodByIdAsync(PaymentMethodByIdRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("PaymentMethodService => Fetching payment method #{Id}", requestModel.PaymentMethodId);
            var item = await _db.TblPaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentMethodId == requestModel.PaymentMethodId);

            if (item == null)
            {
                return new PaymentMethodByIdResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.NotFound,
                    Message = "Payment method not found."
                };
            }

            return new PaymentMethodByIdResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Payment method fetched successfully.",
                Data = MapToModel(item)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentMethodService => Failed to fetch payment method #{Id}", requestModel.PaymentMethodId);
            return new PaymentMethodByIdResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Internal Server Error: " + ex.Message
            };
        }
    }

    public async Task<PaymentMethodPatchResponseModel> UpdatePaymentMethodAsync(PaymentMethodPatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("PaymentMethodService => Updating payment method #{Id}", requestModel.PaymentMethodId);
            var item = await _db.TblPaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == requestModel.PaymentMethodId);
            if (item == null)
            {
                return new PaymentMethodPatchResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.NotFound,
                    Message = "Payment method not found."
                };
            }

            if (requestModel.AccountName != null) item.AccountName = requestModel.AccountName.Trim();
            if (requestModel.AccountNumber != null) item.AccountNumber = requestModel.AccountNumber.Trim();
            if (requestModel.Description != null) item.Description = requestModel.Description.Trim();
            if (requestModel.IsActive.HasValue) item.IsActive = requestModel.IsActive.Value;
            item.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            _logger.LogInformation("PaymentMethodService => Payment method #{Id} updated successfully", item.PaymentMethodId);
            return new PaymentMethodPatchResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = $"{item.PaymentName} updated successfully.",
                Data = MapToModel(item)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentMethodService => Failed to update payment method #{Id}", requestModel.PaymentMethodId);
            return new PaymentMethodPatchResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to update payment method: " + ex.Message
            };
        }
    }

    public async Task<PaymentMethodToggleResponseModel> ToggleStatusAsync(PaymentMethodToggleRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("PaymentMethodService => Toggling payment method #{Id} status to {IsActive}", requestModel.PaymentMethodId, requestModel.IsActive);
            var item = await _db.TblPaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == requestModel.PaymentMethodId);
            if (item == null)
            {
                return new PaymentMethodToggleResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.NotFound,
                    Message = "Payment method not found."
                };
            }

            item.IsActive = requestModel.IsActive;
            item.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            string statusText = requestModel.IsActive ? "enabled" : "disabled";
            _logger.LogInformation("PaymentMethodService => Payment method #{Id} {Status}", requestModel.PaymentMethodId, statusText);
            return new PaymentMethodToggleResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = $"{item.PaymentName} has been {statusText}.",
                Data = MapToModel(item)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentMethodService => Failed to toggle payment method #{Id}", requestModel.PaymentMethodId);
            return new PaymentMethodToggleResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to update status: " + ex.Message
            };
        }
    }

    private static PaymentMethodModel MapToModel(TblPaymentMethod p)
    {
        return new PaymentMethodModel
        {
            PaymentMethodId = p.PaymentMethodId,
            PaymentCode = p.PaymentCode,
            PaymentName = p.PaymentName,
            AccountName = p.AccountName,
            AccountNumber = p.AccountNumber,
            BadgeCode = p.BadgeCode,
            BadgeBg = p.BadgeBg,
            Description = p.Description,
            IsSlipRequired = p.IsSlipRequired,
            IsActive = p.IsActive,
            DisplayOrder = p.DisplayOrder,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
