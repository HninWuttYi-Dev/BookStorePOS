using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Book;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.Book;

public class BookService : IBookService
{
    private readonly AppDbContext _db;
    private readonly ILogger<BookService> _logger;

    public BookService(AppDbContext db, ILogger<BookService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BookListResponseModel> GetBooksAsync(BookListRequestModel requestModel)
    {
        _logger.LogInformation("Get Books Async => Fetching all books");
        try
        {
            var query = _db.TblBooks
                    .AsNoTracking()
                    .Where(b => !b.IsDeleted);

            //apply filters if the user sent a value
            if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
            {
                var queryTerm = requestModel.SearchQuery.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(queryTerm) ||
                                         b.Author.ToLower().Contains(queryTerm) ||
                                         (b.Isbn != null && b.Isbn.ToLower().Contains(queryTerm)));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                query = query.Where(b => b.Title.Contains(requestModel.Title));
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                query = query.Where(b => b.Author.Contains(requestModel.Author));
            }

            if (requestModel.AuthorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == requestModel.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                query = query.Where(b => b.Genre.Contains(requestModel.Genre));
            }

            if (requestModel.GenreId.HasValue)
            {
                query = query.Where(b => b.GenreId == requestModel.GenreId);
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                query = query.Where(b => b.Isbn != null && b.Isbn.Contains(requestModel.Isbn));
            }
            //calculating pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if(totalPages == 0) totalPages = 1;
            var lst = await query.OrderByDescending(b => b.BookId)
                            .Skip((requestModel.Page -1) * requestModel.Limit)
                            .Take(requestModel.Limit)
                            .ToListAsync();
            List<BookModel> books = new List<BookModel>();
            foreach (var item in lst)
            {
                books.Add(new BookModel
                {
                    BookId = item.BookId,
                    Isbn = item.Isbn,
                    Title = item.Title,
                    Author = item.Author,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted,
                    CoverImageUrl = item.CoverImageUrl
                });
            }
            _logger.LogInformation("Get Books Async => Books fetched successfully");
            return new BookListResponseModel
            {
                isSuccess = true,
                Message = "Books fetched successfully",
                Data = books,
                Page = requestModel.Page,
                Limit =requestModel.Limit,
                Count = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Books Async => Failed to fetch books {Message}", ex.Message);
            return new BookListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch books: " + ex.Message
            };
        }
    }

    public async Task<BookByIdResponseModel> GetBookAsync(BookByIdRequestModel requestModel)
    {
        _logger.LogInformation("Get Book ById Async => Fetching book by Id");
        try
        {
            var item = await _db.TblBooks
                        .AsNoTracking()
                        .FirstOrDefaultAsync
                        (x =>
                        x.BookId == requestModel.BookId
                        &&
                        !x.IsDeleted);
            if (item is null)
            {
                _logger.LogWarning("Get Book ById Async => Book is not found");
                return new BookByIdResponseModel
                {
                    isSuccess = false,
                    Message = "Book is not found"
                };
            }
            _logger.LogInformation("Get Book ById Async => Book fetched successfully");
            return new BookByIdResponseModel
            {
                isSuccess = true,
                Message = "Book fetched successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Isbn = item.Isbn,
                    Title = item.Title,
                    Author = item.Author,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted,
                    CoverImageUrl = item.CoverImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Book ById Async => Failed to fetch book {Message}", ex.Message);
            return new BookByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch book: " + ex.Message
            };
        }
    }

    public async Task<BookCreateResponseModel> CreateBookAsync(BookCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Book Async => Creating book");
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.Title))
            {
                _logger.LogWarning("Create Book Async => Title is required");
                return new BookCreateResponseModel { isSuccess = false, Message = "Title is required." };
            }
            if ((!requestModel.AuthorId.HasValue || requestModel.AuthorId.Value <= 0) && string.IsNullOrWhiteSpace(requestModel.Author))
            {
                _logger.LogWarning("Create Book Async => Author is required");
                return new BookCreateResponseModel { isSuccess = false, Message = "Author is required." };
            }
            if ((!requestModel.GenreId.HasValue || requestModel.GenreId.Value <= 0) && string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                _logger.LogWarning("Create Book Async => Genre is required");
                return new BookCreateResponseModel { isSuccess = false, Message = "Genre is required." };
            }
            if (requestModel.ReorderLevel < 0)
            {
                _logger.LogWarning("Create Book Async => ReorderLevel cannot be negative");
                return new BookCreateResponseModel { isSuccess = false, Message = "ReorderLevel cannot be negative." };
            }
            if (requestModel.Price <= 0)
            {
                _logger.LogWarning("Create Book Async => Price must be greater than 0");
                return new BookCreateResponseModel
                {
                    isSuccess = false,
                    Message = "Price must be greater than 0."
                };
            }

            string authorName = requestModel.Author ?? "";
            if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value > 0)
            {
                var authorEntity = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorId == requestModel.AuthorId);
                if (authorEntity != null) authorName = authorEntity.AuthorName;
                else return new BookCreateResponseModel { isSuccess = false, Message = "Invalid Author." };
            }
            else if (!string.IsNullOrWhiteSpace(authorName))
            {
                var existingAuthor = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorName.ToLower() == authorName.ToLower() && !a.IsDeleted);
                if (existingAuthor != null)
                {
                    requestModel.AuthorId = existingAuthor.AuthorId;
                    authorName = existingAuthor.AuthorName;
                }
                else
                {
                    var newAuthor = new TblAuthor
                    {
                        AuthorName = authorName,
                        CreateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.TblAuthors.Add(newAuthor);
                    await _db.SaveChangesAsync();
                    requestModel.AuthorId = newAuthor.AuthorId;
                }
            }
            else
            {
                requestModel.AuthorId = null;
            }

            string genreName = requestModel.Genre ?? "";
            if (requestModel.GenreId.HasValue && requestModel.GenreId.Value > 0)
            {
                var genreEntity = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreId == requestModel.GenreId);
                if (genreEntity != null) genreName = genreEntity.GenreName;
                else return new BookCreateResponseModel { isSuccess = false, Message = "Invalid Genre." };
            }
            else if (!string.IsNullOrWhiteSpace(genreName))
            {
                var existingGenre = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreName.ToLower() == genreName.ToLower() && !g.IsDeleted);
                if (existingGenre != null)
                {
                    requestModel.GenreId = existingGenre.GenreId;
                    genreName = existingGenre.GenreName;
                }
                else
                {
                    var newGenre = new TblGenre
                    {
                        GenreName = genreName,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.TblGenres.Add(newGenre);
                    await _db.SaveChangesAsync();
                    requestModel.GenreId = newGenre.GenreId;
                }
            }
            else
            {
                requestModel.GenreId = null;
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                {
                    _logger.LogWarning("Create Book Async => ISBN must be 10 or 13 digits");
                    return new BookCreateResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." };
                }

                bool isbnExists = await _db.TblBooks
                    .AnyAsync(b => b.Isbn == requestModel.Isbn && !b.IsDeleted);

                if (isbnExists)
                {
                    _logger.LogWarning("Create Book Async => A book with this ISBN already exists");
                    return new BookCreateResponseModel
                    {
                        isSuccess = false,
                        Message = "A book with this ISBN already exists."
                    };
                }
            }
            if (requestModel.StockQuantity < 0)
            {
                _logger.LogWarning("Create Book Async => StockQuantity cannot be negative");
                return new BookCreateResponseModel
                {
                    isSuccess = false,
                    Message = "StockQuantity cannot be negative."
                };
            }

            TblBook book = new TblBook
            {
                Title = requestModel.Title,
                Isbn = requestModel.Isbn,
                Author = authorName,
                AuthorId = requestModel.AuthorId,
                Genre = genreName,
                GenreId = requestModel.GenreId,
                Description = requestModel.Description,
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                ReorderLevel = requestModel.ReorderLevel,
                CoverImageUrl = requestModel.CoverImageUrl,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBooks.Add(book);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Create Book Async => Book is created successfully");
            return new BookCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new book successfully",
                Data = new BookModel
                {
                    BookId = book.BookId,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    Author = book.Author,
                    AuthorId = book.AuthorId,
                    Genre = book.Genre,
                    GenreId = book.GenreId,
                    Description = book.Description,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    ReorderLevel = book.ReorderLevel,
                    IsDeleted = book.IsDeleted,
                    CoverImageUrl = book.CoverImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Create Book Async => Failed to create book {Message}", ex.Message);
            return new BookCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create book: " + ex.Message
            };
        }
    }

    public async Task<BookPatchResponseModel> UpdateBookAsync(BookPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Book Async => Updating book");
        try
        {
            var item = await _db.TblBooks
                        .FirstOrDefaultAsync(x =>
                        x.BookId == requestModel.BookId
                        &&
                        !x.IsDeleted);
            if (item is null)
            {
                _logger.LogWarning("Update Book Async => Book doesn't exist");
                return new BookPatchResponseModel
                {
                    isSuccess = false,
                    Message = "Book doesn't exist"
                };
            }

            if (requestModel.Title != null && string.IsNullOrWhiteSpace(requestModel.Title))
            {
                _logger.LogWarning("Update Book Async => Title is required if provided");
                return new BookPatchResponseModel { isSuccess = false, Message = "Title cannot be empty." };
            }
            if (requestModel.ReorderLevel.HasValue && requestModel.ReorderLevel.Value < 0)
            {
                _logger.LogWarning("Update Book Async => ReorderLevel cannot be negative");
                return new BookPatchResponseModel { isSuccess = false, Message = "ReorderLevel cannot be negative." };
            }
            if (requestModel.Price.HasValue && requestModel.Price.Value <= 0)
            {
                _logger.LogWarning("Update Book Async => Price must be greater than 0");
                return new BookPatchResponseModel { isSuccess = false, Message = "Price must be greater than 0." };
            }
            if (requestModel.StockQuantity.HasValue && requestModel.StockQuantity.Value < 0)
            {
                _logger.LogWarning("Update Book Async => StockQuantity cannot be negative");
                return new BookPatchResponseModel { isSuccess = false, Message = "StockQuantity cannot be negative." };
            }
            
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                {
                    _logger.LogWarning("Update Book Async => ISBN must be 10 or 13 digits");
                    return new BookPatchResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." };
                }

                bool isbnExists = await _db.TblBooks
                    .AnyAsync(b => b.Isbn == requestModel.Isbn
                                && b.BookId != requestModel.BookId
                                && !b.IsDeleted);

                if (isbnExists)
                {
                    _logger.LogWarning("Update Book Async => A book with this ISBN already exists");
                    return new BookPatchResponseModel
                    {
                        isSuccess = false,
                        Message = "A book with this ISBN already exists."
                    };
                }
            }

            string authorName = requestModel.Author ?? item.Author;
            if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value > 0)
            {
                var authorEntity = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorId == requestModel.AuthorId);
                if (authorEntity != null) authorName = authorEntity.AuthorName;
                else return new BookPatchResponseModel { isSuccess = false, Message = "Invalid Author." };
            }
            else if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value <= 0 && !string.IsNullOrWhiteSpace(requestModel.Author))
            {
                var existingAuthor = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorName.ToLower() == requestModel.Author.ToLower() && !a.IsDeleted);
                if (existingAuthor != null)
                {
                    requestModel.AuthorId = existingAuthor.AuthorId;
                    authorName = existingAuthor.AuthorName;
                }
                else
                {
                    var newAuthor = new TblAuthor
                    {
                        AuthorName = requestModel.Author,
                        CreateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.TblAuthors.Add(newAuthor);
                    await _db.SaveChangesAsync();
                    requestModel.AuthorId = newAuthor.AuthorId;
                    authorName = newAuthor.AuthorName;
                }
            }
            else if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value <= 0)
            {
                requestModel.AuthorId = null;
            }

            string genreName = requestModel.Genre ?? item.Genre;
            if (requestModel.GenreId.HasValue && requestModel.GenreId.Value > 0)
            {
                var genreEntity = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreId == requestModel.GenreId);
                if (genreEntity != null) genreName = genreEntity.GenreName;
                else return new BookPatchResponseModel { isSuccess = false, Message = "Invalid Genre." };
            }
            else if (requestModel.GenreId.HasValue && requestModel.GenreId.Value <= 0 && !string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                var existingGenre = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreName.ToLower() == requestModel.Genre.ToLower() && !g.IsDeleted);
                if (existingGenre != null)
                {
                    requestModel.GenreId = existingGenre.GenreId;
                    genreName = existingGenre.GenreName;
                }
                else
                {
                    var newGenre = new TblGenre
                    {
                        GenreName = requestModel.Genre,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _db.TblGenres.Add(newGenre);
                    await _db.SaveChangesAsync();
                    requestModel.GenreId = newGenre.GenreId;
                    genreName = newGenre.GenreName;
                }
            }
            else if (requestModel.GenreId.HasValue && requestModel.GenreId.Value <= 0)
            {
                requestModel.GenreId = null;
            }

            if (!string.IsNullOrEmpty(requestModel.Isbn)) item.Isbn = requestModel.Isbn;
            if (!string.IsNullOrEmpty(requestModel.Title)) item.Title = requestModel.Title;
            
            item.Author = authorName;
            if (requestModel.AuthorId.HasValue) item.AuthorId = requestModel.AuthorId;
            else if (!string.IsNullOrEmpty(requestModel.Author)) item.AuthorId = null; // Unset ID if they manually passed a string

            item.Genre = genreName;
            if (requestModel.GenreId.HasValue) item.GenreId = requestModel.GenreId;
            else if (!string.IsNullOrEmpty(requestModel.Genre)) item.GenreId = null; // Unset ID if they manually passed a string

            if (requestModel.Description != null) item.Description = requestModel.Description;
            if (requestModel.Price.HasValue) item.Price = requestModel.Price.Value;
            if (requestModel.ReorderLevel.HasValue) item.ReorderLevel = requestModel.ReorderLevel.Value;
            if (requestModel.StockQuantity.HasValue) item.StockQuantity = requestModel.StockQuantity.Value;
            if (requestModel.CoverImageUrl != null) item.CoverImageUrl = requestModel.CoverImageUrl;

            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Book Async => Book is updated successfully");
            return new BookPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated book successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Isbn = item.Isbn,
                    Title = item.Title,
                    Author = item.Author,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted,
                    CoverImageUrl = item.CoverImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Book Async => Failed to update book {Message}", ex.Message);
            return new BookPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update book: " + ex.Message
            };
        }
    }

    public async Task<BookDeleteResponseModel> DeleteBookAsync(BookDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Book Async => Deleting book");
        try
        {
            var item = await _db.TblBooks
                    .FirstOrDefaultAsync(x =>
                    x.BookId == requestModel.BookId);
            if (item is null)
            {
                _logger.LogWarning("Delete Book Async => Book is not found");
                return new BookDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Book is not found"
                };
            }

            if (item.IsDeleted)
            {
                _logger.LogWarning("Delete Book Async => Book is already deleted");
                return new BookDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Book is already deleted."
                };
            }

            // Soft delete
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Book Async => Book is deleted successfully");
            return new BookDeleteResponseModel
            {
                isSuccess = true,
                Message = "Book is deleted successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Isbn = item.Isbn,
                    Title = item.Title,
                    Author = item.Author,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted,
                    CoverImageUrl = item.CoverImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Book Async => Failed to delete book {Message}", ex.Message);
            return new BookDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete book: " + ex.Message
            };
        }
    }

    public async Task<BookListResponseModel> GetLowStockBooksAsync()
    {
        _logger.LogInformation("Get Low Stock Books Async => Fetching low stock books");
        try
        {
            var query = _db.TblBooks
                .AsNoTracking()
                .Where(b => !b.IsDeleted && b.StockQuantity <= b.ReorderLevel);

            var lst = await query.ToListAsync();
            List<BookModel> books = new List<BookModel>();
            foreach (var item in lst)
            {
                books.Add(new BookModel
                {
                    BookId = item.BookId,
                    Isbn = item.Isbn,
                    Title = item.Title,
                    Author = item.Author,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted,
                    CoverImageUrl = item.CoverImageUrl
                });
            }

            _logger.LogInformation("Get Low Stock Books Async => Low stock books fetched successfully");
            return new BookListResponseModel
            {
                isSuccess = true,
                Message = "Low stock books fetched successfully",
                Data = books
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get Low Stock Books Async => Failed to fetch low stock books");
            return new BookListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch low stock books: " + ex.Message
            };
        }
    }
}
