using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Genre;
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
                isSuccess = true,
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
                isSuccess = false,
                Message = "Failed to fetch genres: " + ex.Message
            };
        }
    }

    public async Task<GenreByIdResponseModel> GetGenreByIdAsync(GenreByIdRequestModel requestModel)
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
                return new GenreByIdResponseModel
                {
                    isSuccess = false,
                    Message = "Genre is not found"
                };
            }
            
            _logger.LogInformation("Get Genre ById Async => Genre fetched successfully");
            return new GenreByIdResponseModel
            {
                isSuccess = true,
                Message = "Genre fetched successfully",
                Data = new GenreModel
                {
                    GenreId = item.GenreId,
                    GenreName = item.GenreName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Genre ById Async => Failed to fetch genre {Message}", ex.Message);
            return new GenreByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch genre: " + ex.Message
            };
        }
    }

    public async Task<GenreCreateResponseModel> CreateGenreAsync(GenreCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Genre Async => Creating genre");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                _logger.LogWarning("Create Genre Async => Genre name is required");
                return new GenreCreateResponseModel { isSuccess = false, Message = "Genre name is required." };
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
            return new GenreCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new genre successfully",
                Data = new GenreModel
                {
                    GenreId = genre.GenreId,
                    GenreName = genre.GenreName,
                    IsDeleted = genre.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Genre Async => Failed to create genre {Message}", ex.Message);
            return new GenreCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create genre: " + ex.Message
            };
        }
    }

    public async Task<GenrePatchResponseModel> UpdateGenreAsync(GenrePatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Genre Async => Updating genre");
        try
        {
            var item = await _db.TblGenres
                        .FirstOrDefaultAsync(x => x.GenreId == requestModel.GenreId && !x.IsDeleted);
                        
            if (item is null)
            {
                _logger.LogWarning("Update Genre Async => Genre doesn't exist");
                return new GenrePatchResponseModel
                {
                    isSuccess = false,
                    Message = "Genre doesn't exist"
                };
            }

            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                _logger.LogWarning("Update Genre Async => Genre name is required");
                return new GenrePatchResponseModel { isSuccess = false, Message = "Genre name is required." };
            }

            item.GenreName = requestModel.GenreName;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Genre Async => Genre is updated successfully");
            return new GenrePatchResponseModel
            {
                isSuccess = true,
                Message = "Updated genre successfully",
                Data = new GenreModel
                {
                    GenreId = item.GenreId,
                    GenreName = item.GenreName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Genre Async => Failed to update genre {Message}", ex.Message);
            return new GenrePatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update genre: " + ex.Message
            };
        }
    }

    public async Task<GenreDeleteResponseModel> DeleteGenreAsync(GenreDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Genre Async => Deleting genre");
        try
        {
            var item = await _db.TblGenres
                    .FirstOrDefaultAsync(x => x.GenreId == requestModel.GenreId);
                    
            if (item is null)
            {
                _logger.LogWarning("Delete Genre Async => Genre is not found");
                return new GenreDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Genre is not found"
                };
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Genre Async => Genre is already deleted");
                return new GenreDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Genre is already deleted."
                };
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Genre Async => Genre is deleted successfully");
            return new GenreDeleteResponseModel
            {
                isSuccess = true,
                Message = "Genre is deleted successfully",
                Data = new GenreModel
                {
                    GenreId = item.GenreId,
                    GenreName = item.GenreName,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Genre Async => Failed to delete genre {Message}", ex.Message);
            return new GenreDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete genre: " + ex.Message
            };
        }
    }
}
