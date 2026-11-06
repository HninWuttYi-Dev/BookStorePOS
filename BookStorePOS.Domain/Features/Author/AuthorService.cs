using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Author;
using BookStorePOS.Shared.Models.Common;
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
                    .Where(a => !a.IsDeleted && a.TblBooks.Any(b => !b.IsDeleted));

            if (!string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                query = query.Where(a => a.AuthorName.Contains(requestModel.AuthorName));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if (totalPages == 0) totalPages = 1;
            
            var lst = await query
                            .Select(a => new
                            {
                                a.AuthorId,
                                a.AuthorName,
                                a.IsDeleted,
                                BookCount = a.TblBooks.Count(b => !b.IsDeleted),
                                SampleBooks = a.TblBooks
                                    .Where(b => !b.IsDeleted)
                                    .OrderByDescending(b => b.BookId)
                                    .Take(10)
                                    .Select(b => new AuthorBookPreviewModel
                                    {
                                        BookId = b.BookId,
                                        Title = b.Title,
                                        Author = a.AuthorName,
                                        CoverImageUrl = b.TblBookEditions
                                            .Where(e => !e.IsDeleted && !string.IsNullOrEmpty(e.CoverImageUrl))
                                            .Select(e => e.CoverImageUrl)
                                            .FirstOrDefault(),
                                        Price = b.TblBookEditions
                                            .Where(e => !e.IsDeleted)
                                            .Select(e => e.Price)
                                            .FirstOrDefault()
                                    })
                                    .ToList()
                            })
                            .OrderBy(a => a.AuthorName)
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
                    IsDeleted = item.IsDeleted,
                    BookCount = item.BookCount,
                    SampleBooks = item.SampleBooks
                });
            }
            
            _logger.LogInformation("Get Authors Async => Authors fetched successfully");
            return new AuthorListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
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
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch authors: " + ex.Message
            };
        }
    }

    public async Task<AuthorCursorResponseModel> GetAuthorsCursorAsync(AuthorCursorRequestModel requestModel)
    {
        _logger.LogInformation("Get Authors Cursor Async => Cursor: {Cursor}, Limit: {Limit}", requestModel.Cursor, requestModel.Limit);
        try
        {
            var query = _db.TblAuthors
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted && a.TblBooks.Any(b => !b.IsDeleted));

            if (!string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                query = query.Where(a => a.AuthorName.Contains(requestModel.AuthorName));
            }

            var totalCount = await query.CountAsync();

            if (requestModel.Cursor.HasValue && requestModel.Cursor.Value > 0)
            {
                query = query.Where(a => a.AuthorId > requestModel.Cursor.Value);
            }

            int takeCount = requestModel.Limit <= 0 ? 6 : requestModel.Limit;

            var rawList = await query
                .OrderBy(a => a.AuthorId)
                .Take(takeCount + 1)
                .Select(a => new
                {
                    a.AuthorId,
                    a.AuthorName,
                    a.IsDeleted,
                    BookCount = a.TblBooks.Count(b => !b.IsDeleted),
                    SampleBooks = a.TblBooks
                        .Where(b => !b.IsDeleted)
                        .OrderByDescending(b => b.BookId)
                        .Take(12)
                        .Select(b => new AuthorBookPreviewModel
                        {
                            BookId = b.BookId,
                            Title = b.Title,
                            Author = a.AuthorName,
                            CoverImageUrl = b.TblBookEditions
                                .Where(e => !e.IsDeleted && !string.IsNullOrEmpty(e.CoverImageUrl))
                                .Select(e => e.CoverImageUrl)
                                .FirstOrDefault(),
                            Price = b.TblBookEditions
                                .Where(e => !e.IsDeleted)
                                .Select(e => e.Price)
                                .FirstOrDefault()
                        })
                        .ToList()
                })
                .ToListAsync();

            bool hasNextPage = rawList.Count > takeCount;
            if (hasNextPage)
            {
                rawList.RemoveAt(rawList.Count - 1);
            }

            int? nextCursor = hasNextPage && rawList.Any() ? rawList.Last().AuthorId : null;

            List<AuthorModel> authors = rawList.Select(item => new AuthorModel
            {
                AuthorId = item.AuthorId,
                AuthorName = item.AuthorName,
                IsDeleted = item.IsDeleted,
                BookCount = item.BookCount,
                SampleBooks = item.SampleBooks
            }).ToList();

            return new AuthorCursorResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Authors fetched successfully via cursor",
                Data = authors,
                NextCursor = nextCursor,
                HasNextPage = hasNextPage,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthorsCursorAsync => Exception occurred");
            return new AuthorCursorResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch authors via cursor: " + ex.Message
            };
        }
    }

    public async Task<Result<AuthorModel>> GetAuthorByIdAsync(AuthorByIdRequestModel requestModel)
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
                return Result<AuthorModel>.NotFound("Author is not found");
            }
            
            _logger.LogInformation("Get Author ById Async => Author fetched successfully");
            return Result<AuthorModel>.Success(new AuthorModel
            {
                AuthorId = item.AuthorId,
                AuthorName = item.AuthorName,
                IsDeleted = item.IsDeleted
            }, "Author fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Author ById Async => Failed to fetch author {Message}", ex.Message);
            return Result<AuthorModel>.SystemError("Failed to fetch author: " + ex.Message);
        }
    }

    public async Task<Result<AuthorModel>> CreateAuthorAsync(AuthorCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Author Async => Creating author");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                _logger.LogWarning("Create Author Async => Author name is required");
                return Result<AuthorModel>.ValidationError("Author name is required.");
            }

            var exists = await _db.TblAuthors.AnyAsync(a => a.AuthorName.ToLower() == requestModel.AuthorName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Create Author Async => Author name already exists");
                return Result<AuthorModel>.Conflict("Author already exists.");
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
            return Result<AuthorModel>.Success(new AuthorModel
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
                IsDeleted = author.IsDeleted
            }, "Created new author successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Author Async => Failed to create author {Message}", ex.Message);
            return Result<AuthorModel>.SystemError("Failed to create author: " + ex.Message);
        }
    }

    public async Task<Result<AuthorModel>> UpdateAuthorAsync(AuthorPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Author Async => Updating author");
        try
        {
            var item = await _db.TblAuthors
                        .FirstOrDefaultAsync(x => x.AuthorId == requestModel.AuthorId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Update Author Async => Author doesn't exist");
                return Result<AuthorModel>.NotFound("Author doesn't exist");
            }

            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                _logger.LogWarning("Update Author Async => Author name is required");
                return Result<AuthorModel>.ValidationError("Author name is required.");
            }

            var exists = await _db.TblAuthors.AnyAsync(a => a.AuthorId != requestModel.AuthorId && a.AuthorName.ToLower() == requestModel.AuthorName.ToLower() && !a.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Update Author Async => Author name already exists");
                return Result<AuthorModel>.Conflict("Author already exists.");
            }

            item.AuthorName = requestModel.AuthorName;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Author Async => Author is updated successfully");
            return Result<AuthorModel>.Success(new AuthorModel
            {
                AuthorId = item.AuthorId,
                AuthorName = item.AuthorName,
                IsDeleted = item.IsDeleted
            }, "Updated author successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Author Async => Failed to update author {Message}", ex.Message);
            return Result<AuthorModel>.SystemError("Failed to update author: " + ex.Message);
        }
    }

    public async Task<Result<AuthorModel>> DeleteAuthorAsync(AuthorDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Author Async => Deleting author");
        try
        {
            var item = await _db.TblAuthors
                    .FirstOrDefaultAsync(x => x.AuthorId == requestModel.AuthorId);
                    
            if (item is null)
            {
                _logger.LogWarning("Delete Author Async => Author is not found");
                return Result<AuthorModel>.NotFound("Author is not found");
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Author Async => Author is already deleted");
                return Result<AuthorModel>.Conflict("Author is already deleted.");
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Author Async => Author is deleted successfully");
            return Result<AuthorModel>.Success(new AuthorModel
            {
                AuthorId = item.AuthorId,
                AuthorName = item.AuthorName,
                IsDeleted = item.IsDeleted
            }, "Author is deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Author Async => Failed to delete author {Message}", ex.Message);
            return Result<AuthorModel>.SystemError("Failed to delete author: " + ex.Message);
        }
    }
}
