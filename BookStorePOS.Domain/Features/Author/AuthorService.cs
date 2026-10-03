using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Author;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.Author;

public class AuthorService : IAuthorService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(AppDbContext db, ILogger<AuthorService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<AuthorListResponseModel> GetAuthorsAsync(AuthorListRequestModel requestModel)
    {
        _logger.LogInformation("Get Authors Async => Fetching all authors");
        try
        {
            var query = _db.TblAuthors
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                query = query.Where(a => a.AuthorName.Contains(requestModel.AuthorName));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if (totalPages == 0) totalPages = 1;
            
            var lst = await query.OrderBy(a => a.AuthorName)
                            .Skip((requestModel.Page - 1) * requestModel.Limit)
                            .Take(requestModel.Limit)
                            .ToListAsync();
                            
            List<AuthorModel> authors = new List<AuthorModel>();
            foreach (var item in lst)
            {
                authors.Add(new AuthorModel
                {
                    AuthorId = item.AuthorId,
                    AuthorName = item.AuthorName,
                    IsDeleted = item.IsDeleted
                });
            }
            
            _logger.LogInformation("Get Authors Async => Authors fetched successfully");
            return new AuthorListResponseModel
            {
                isSuccess = true,
                Message = "Authors fetched successfully",
                Data = authors,
                Page = requestModel.Page,
                Limit = requestModel.Limit,
                Count = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Authors Async => Failed to fetch authors {Message}", ex.Message);
            return new AuthorListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch authors: " + ex.Message
            };
        }
    }

    public async Task<AuthorByIdResponseModel> GetAuthorByIdAsync(AuthorByIdRequestModel requestModel)
    {
        _logger.LogInformation("Get Author ById Async => Fetching author by Id");
        try
        {
            var item = await _db.TblAuthors
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.AuthorId == requestModel.AuthorId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Get Author ById Async => Author is not found");
                return new AuthorByIdResponseModel
                {
                    isSuccess = false,
                    Message = "Author is not found"
                };
            }
            
            _logger.LogInformation("Get Author ById Async => Author fetched successfully");
            return new AuthorByIdResponseModel
            {
                isSuccess = true,
                Message = "Author fetched successfully",
                Data = new AuthorModel
                {
                    AuthorId = item.AuthorId,
                    AuthorName = item.AuthorName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Author ById Async => Failed to fetch author {Message}", ex.Message);
            return new AuthorByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch author: " + ex.Message
            };
        }
    }

    public async Task<AuthorCreateResponseModel> CreateAuthorAsync(AuthorCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Author Async => Creating author");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                _logger.LogWarning("Create Author Async => Author name is required");
                return new AuthorCreateResponseModel { isSuccess = false, Message = "Author name is required." };
            }

            var exists = await _db.TblAuthors.AnyAsync(a => a.AuthorName.ToLower() == requestModel.AuthorName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Create Author Async => Author name already exists");
                return new AuthorCreateResponseModel { isSuccess = false, Message = "Author already exists." };
            }

            var author = new TblAuthor
            {
                AuthorName = requestModel.AuthorName,
                IsDeleted = false,
                CreateAt = DateTime.Now
            };
            
            _db.TblAuthors.Add(author);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Create Author Async => Author is created successfully");
            return new AuthorCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new author successfully",
                Data = new AuthorModel
                {
                    AuthorId = author.AuthorId,
                    AuthorName = author.AuthorName,
                    IsDeleted = author.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Author Async => Failed to create author {Message}", ex.Message);
            return new AuthorCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create author: " + ex.Message
            };
        }
    }

    public async Task<AuthorPatchResponseModel> UpdateAuthorAsync(AuthorPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Author Async => Updating author");
        try
        {
            var item = await _db.TblAuthors
                        .FirstOrDefaultAsync(x => x.AuthorId == requestModel.AuthorId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Update Author Async => Author doesn't exist");
                return new AuthorPatchResponseModel
                {
                    isSuccess = false,
                    Message = "Author doesn't exist"
                };
            }

            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                _logger.LogWarning("Update Author Async => Author name is required");
                return new AuthorPatchResponseModel { isSuccess = false, Message = "Author name is required." };
            }

            var exists = await _db.TblAuthors.AnyAsync(a => a.AuthorId != requestModel.AuthorId && a.AuthorName.ToLower() == requestModel.AuthorName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Update Author Async => Author name already exists");
                return new AuthorPatchResponseModel { isSuccess = false, Message = "Author already exists." };
            }

            item.AuthorName = requestModel.AuthorName;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Author Async => Author is updated successfully");
            return new AuthorPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated author successfully",
                Data = new AuthorModel
                {
                    AuthorId = item.AuthorId,
                    AuthorName = item.AuthorName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Author Async => Failed to update author {Message}", ex.Message);
            return new AuthorPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update author: " + ex.Message
            };
        }
    }

    public async Task<AuthorDeleteResponseModel> DeleteAuthorAsync(AuthorDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Author Async => Deleting author");
        try
        {
            var item = await _db.TblAuthors
                    .FirstOrDefaultAsync(x => x.AuthorId == requestModel.AuthorId);
                    
            if (item is null)
            {
                _logger.LogWarning("Delete Author Async => Author is not found");
                return new AuthorDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Author is not found"
                };
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Author Async => Author is already deleted");
                return new AuthorDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Author is already deleted."
                };
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Author Async => Author is deleted successfully");
            return new AuthorDeleteResponseModel
            {
                isSuccess = true,
                Message = "Author is deleted successfully",
                Data = new AuthorModel
                {
                    AuthorId = item.AuthorId,
                    AuthorName = item.AuthorName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Author Async => Failed to delete author {Message}", ex.Message);
            return new AuthorDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete author: " + ex.Message
            };
        }
    }
}
