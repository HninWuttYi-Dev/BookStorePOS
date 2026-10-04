using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Edition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.Edition;

public class EditionService : IEditionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<EditionService> _logger;

    public EditionService(AppDbContext db, ILogger<EditionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<EditionListResponseModel> GetEditionsAsync(EditionListRequestModel requestModel)
    {
        _logger.LogInformation("Get Editions Async => Fetching all editions");
        try
        {
            var query = _db.TblEditions
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                query = query.Where(a => a.EditionName.Contains(requestModel.EditionName));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if (totalPages == 0) totalPages = 1;
            
            var lst = await query.OrderBy(a => a.EditionName)
                            .Skip((requestModel.Page - 1) * requestModel.Limit)
                            .Take(requestModel.Limit)
                            .ToListAsync();
                            
            List<EditionModel> editions = new List<EditionModel>();
            foreach (var item in lst)
            {
                editions.Add(new EditionModel
                {
                    EditionId = item.EditionId,
                    EditionName = item.EditionName,
                    IsDeleted = item.IsDeleted
                });
            }
            
            _logger.LogInformation("Get Editions Async => Editions fetched successfully");
            return new EditionListResponseModel
            {
                isSuccess = true,
                Message = "Editions fetched successfully",
                Data = editions,
                Page = requestModel.Page,
                Limit = requestModel.Limit,
                Count = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Editions Async => Failed to fetch editions {Message}", ex.Message);
            return new EditionListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch editions: " + ex.Message
            };
        }
    }

    public async Task<EditionByIdResponseModel> GetEditionByIdAsync(EditionByIdRequestModel requestModel)
    {
        _logger.LogInformation("Get Edition ById Async => Fetching edition by Id");
        try
        {
            var item = await _db.TblEditions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.EditionId == requestModel.EditionId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Get Edition ById Async => Edition is not found");
                return new EditionByIdResponseModel
                {
                    isSuccess = false,
                    Message = "Edition is not found"
                };
            }
            
            _logger.LogInformation("Get Edition ById Async => Edition fetched successfully");
            return new EditionByIdResponseModel
            {
                isSuccess = true,
                Message = "Edition fetched successfully",
                Data = new EditionModel
                {
                    EditionId = item.EditionId,
                    EditionName = item.EditionName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Edition ById Async => Failed to fetch edition {Message}", ex.Message);
            return new EditionByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch edition: " + ex.Message
            };
        }
    }

    public async Task<EditionCreateResponseModel> CreateEditionAsync(EditionCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Edition Async => Creating edition");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                _logger.LogWarning("Create Edition Async => Edition name is required");
                return new EditionCreateResponseModel { isSuccess = false, Message = "Edition name is required." };
            }

            var exists = await _db.TblEditions.AnyAsync(a => a.EditionName.ToLower() == requestModel.EditionName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Create Edition Async => Edition name already exists");
                return new EditionCreateResponseModel { isSuccess = false, Message = "Edition already exists." };
            }

            var edition = new TblEdition
            {
                EditionName = requestModel.EditionName,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            
            _db.TblEditions.Add(edition);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Create Edition Async => Edition is created successfully");
            return new EditionCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new edition successfully",
                Data = new EditionModel
                {
                    EditionId = edition.EditionId,
                    EditionName = edition.EditionName,
                    IsDeleted = edition.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Edition Async => Failed to create edition {Message}", ex.Message);
            return new EditionCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create edition: " + ex.Message
            };
        }
    }

    public async Task<EditionPatchResponseModel> UpdateEditionAsync(EditionPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Edition Async => Updating edition");
        try
        {
            var item = await _db.TblEditions
                        .FirstOrDefaultAsync(x => x.EditionId == requestModel.EditionId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Update Edition Async => Edition doesn't exist");
                return new EditionPatchResponseModel
                {
                    isSuccess = false,
                    Message = "Edition doesn't exist"
                };
            }

            if (string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                _logger.LogWarning("Update Edition Async => Edition name is required");
                return new EditionPatchResponseModel { isSuccess = false, Message = "Edition name is required." };
            }

            var exists = await _db.TblEditions.AnyAsync(a => a.EditionId != requestModel.EditionId && a.EditionName.ToLower() == requestModel.EditionName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Update Edition Async => Edition name already exists");
                return new EditionPatchResponseModel { isSuccess = false, Message = "Edition already exists." };
            }

            item.EditionName = requestModel.EditionName;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Edition Async => Edition is updated successfully");
            return new EditionPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated edition successfully",
                Data = new EditionModel
                {
                    EditionId = item.EditionId,
                    EditionName = item.EditionName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Edition Async => Failed to update edition {Message}", ex.Message);
            return new EditionPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update edition: " + ex.Message
            };
        }
    }

    public async Task<EditionDeleteResponseModel> DeleteEditionAsync(EditionDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Edition Async => Deleting edition");
        try
        {
            var item = await _db.TblEditions
                    .FirstOrDefaultAsync(x => x.EditionId == requestModel.EditionId);
                    
            if (item is null)
            {
                _logger.LogWarning("Delete Edition Async => Edition is not found");
                return new EditionDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Edition is not found"
                };
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Edition Async => Edition is already deleted");
                return new EditionDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Edition is already deleted."
                };
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Edition Async => Edition is deleted successfully");
            return new EditionDeleteResponseModel
            {
                isSuccess = true,
                Message = "Edition is deleted successfully",
                Data = new EditionModel
                {
                    EditionId = item.EditionId,
                    EditionName = item.EditionName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Edition Async => Failed to delete edition {Message}", ex.Message);
            return new EditionDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete edition: " + ex.Message
            };
        }
    }
}
