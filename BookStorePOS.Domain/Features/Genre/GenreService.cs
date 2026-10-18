using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Genre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.Genre;

public class GenreService : IGenreService
{
    private readonly AppDbContext _db;
    private readonly ILogger<GenreService> _logger;

    public GenreService(AppDbContext db, ILogger<GenreService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<GenreListResponseModel> GetGenresAsync(GenreListRequestModel requestModel)
    {
        _logger.LogInformation("Get Genres Async => Fetching all genres");
        try
        {
            var query = _db.TblGenres
                    .AsNoTracking()
                    .Where(g => !g.IsDeleted);

            if (!string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                query = query.Where(g => g.GenreName.Contains(requestModel.GenreName));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if (totalPages == 0) totalPages = 1;
            
            var lst = await query.OrderByDescending(g => g.CreatedAt)
                            .Skip((requestModel.Page - 1) * requestModel.Limit)
                            .Take(requestModel.Limit)
                            .ToListAsync();
                            
            List<GenreModel> genres = new List<GenreModel>();
            foreach (var item in lst)
            {
                genres.Add(new GenreModel
                {
                    GenreId = item.GenreId,
                    GenreName = item.GenreName,
                    IsDeleted = item.IsDeleted
                });
            }
            
            _logger.LogInformation("Get Genres Async => Genres fetched successfully");
            return new GenreListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Genres fetched successfully",
                Data = genres,
                Page = requestModel.Page,
                Limit = requestModel.Limit,
                Count = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Genres Async => Failed to fetch genres {Message}", ex.Message);
            return new GenreListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch genres: " + ex.Message
            };
        }
    }

    public async Task<Result<GenreModel>> GetGenreByIdAsync(GenreByIdRequestModel requestModel)
    {
        _logger.LogInformation("Get Genre ById Async => Fetching genre by Id");
        try
        {
            var item = await _db.TblGenres
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.GenreId == requestModel.GenreId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Get Genre ById Async => Genre is not found");
                return Result<GenreModel>.NotFound("Genre is not found");
            }
            
            _logger.LogInformation("Get Genre ById Async => Genre fetched successfully");
            return Result<GenreModel>.Success(new GenreModel
            {
                GenreId = item.GenreId,
                GenreName = item.GenreName,
                IsDeleted = item.IsDeleted
            }, "Genre fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Genre ById Async => Failed to fetch genre {Message}", ex.Message);
            return Result<GenreModel>.SystemError("Failed to fetch genre: " + ex.Message);
        }
    }

    public async Task<Result<GenreModel>> CreateGenreAsync(GenreCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Genre Async => Creating genre");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                _logger.LogWarning("Create Genre Async => Genre name is required");
                return Result<GenreModel>.ValidationError("Genre name is required.");
            }

            var exists = await _db.TblGenres.AnyAsync(g => g.GenreName.ToLower() == requestModel.GenreName.ToLower() && !g.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Create Genre Async => Genre name already exists");
                return Result<GenreModel>.Conflict("Genre already exists.");
            }

            var genre = new TblGenre
            {
                GenreName = requestModel.GenreName,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            
            _db.TblGenres.Add(genre);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Create Genre Async => Genre is created successfully");
            return Result<GenreModel>.Success(new GenreModel
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
                IsDeleted = genre.IsDeleted
            }, "Created new genre successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Genre Async => Failed to create genre {Message}", ex.Message);
            return Result<GenreModel>.SystemError("Failed to create genre: " + ex.Message);
        }
    }

    public async Task<Result<GenreModel>> UpdateGenreAsync(GenrePatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Genre Async => Updating genre");
        try
        {
            var item = await _db.TblGenres
                        .FirstOrDefaultAsync(x => x.GenreId == requestModel.GenreId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Update Genre Async => Genre doesn't exist");
                return Result<GenreModel>.NotFound("Genre doesn't exist");
            }

            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                _logger.LogWarning("Update Genre Async => Genre name is required");
                return Result<GenreModel>.ValidationError("Genre name is required.");
            }

            var exists = await _db.TblGenres.AnyAsync(g => g.GenreId != requestModel.GenreId && g.GenreName.ToLower() == requestModel.GenreName.ToLower() && !g.IsDeleted);
            if (exists)
            {
                _logger.LogWarning("Update Genre Async => Genre name already exists");
                return Result<GenreModel>.Conflict("Genre already exists.");
            }

            item.GenreName = requestModel.GenreName;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Genre Async => Genre is updated successfully");
            return Result<GenreModel>.Success(new GenreModel
            {
                GenreId = item.GenreId,
                GenreName = item.GenreName,
                IsDeleted = item.IsDeleted
            }, "Updated genre successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Genre Async => Failed to update genre {Message}", ex.Message);
            return Result<GenreModel>.SystemError("Failed to update genre: " + ex.Message);
        }
    }

    public async Task<Result<GenreModel>> DeleteGenreAsync(GenreDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Genre Async => Deleting genre");
        try
        {
            var item = await _db.TblGenres
                    .FirstOrDefaultAsync(x => x.GenreId == requestModel.GenreId);
                    
            if (item is null)
            {
                _logger.LogWarning("Delete Genre Async => Genre is not found");
                return Result<GenreModel>.NotFound("Genre is not found");
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Genre Async => Genre is already deleted");
                return Result<GenreModel>.Conflict("Genre is already deleted.");
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Genre Async => Genre is deleted successfully");
            return Result<GenreModel>.Success(new GenreModel
            {
                GenreId = item.GenreId,
                GenreName = item.GenreName,
                IsDeleted = item.IsDeleted
            }, "Genre is deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Genre Async => Failed to delete genre {Message}", ex.Message);
            return Result<GenreModel>.SystemError("Failed to delete genre: " + ex.Message);
        }
    }
}
