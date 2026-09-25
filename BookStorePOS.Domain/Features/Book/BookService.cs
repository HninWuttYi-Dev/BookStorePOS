using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
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
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                query = query.Where(b => b.Title.Contains(requestModel.Title));
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                query = query.Where(b => b.Author.Contains(requestModel.Author));
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                query = query.Where(b => b.Genre.Contains(requestModel.Genre));
            }

            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                query = query.Where(b => b.Isbn != null && b.Isbn.Contains(requestModel.Isbn));
            }
            //calculating pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if(totalPages == 0) totalPages = 1;
            var lst = await query.OrderByDescending(b => b.CreatedAt)
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
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted
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
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted
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
            if (string.IsNullOrWhiteSpace(requestModel.Author))
            {
                _logger.LogWarning("Create Book Async => Author is required");
                return new BookCreateResponseModel { isSuccess = false, Message = "Author is required." };
            }
            if (string.IsNullOrWhiteSpace(requestModel.Genre))
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
                Author = requestModel.Author,
                Genre = requestModel.Genre,
                Description = requestModel.Description,
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                ReorderLevel = requestModel.ReorderLevel,
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
                    Genre = book.Genre,
                    Description = book.Description,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    ReorderLevel = requestModel.ReorderLevel,
                    IsDeleted = book.IsDeleted
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
                        .AsNoTracking()
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

            if (string.IsNullOrWhiteSpace(requestModel.Title))
            {
                _logger.LogWarning("Update Book Async => Title is required");
                return new BookPatchResponseModel { isSuccess = false, Message = "Title is required." };
            }
            if (string.IsNullOrWhiteSpace(requestModel.Author))
            {
                _logger.LogWarning("Update Book Async => Author is required");
                return new BookPatchResponseModel { isSuccess = false, Message = "Author is required." };
            }
            if (string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                _logger.LogWarning("Update Book Async => Genre is required");
                return new BookPatchResponseModel { isSuccess = false, Message = "Genre is required." };
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
            if (!string.IsNullOrEmpty(requestModel.Isbn)) item.Isbn = requestModel.Isbn;
            if (!string.IsNullOrEmpty(requestModel.Title)) item.Title = requestModel.Title;
            if (!string.IsNullOrEmpty(requestModel.Author)) item.Author = requestModel.Author;
            if (!string.IsNullOrEmpty(requestModel.Genre)) item.Genre = requestModel.Genre;
            if (requestModel.Description != null) item.Description = requestModel.Description;
            if (requestModel.Price.HasValue) item.Price = requestModel.Price.Value;
            if (requestModel.ReorderLevel.HasValue) item.ReorderLevel = requestModel.ReorderLevel.Value;
            if (requestModel.StockQuantity.HasValue) item.StockQuantity = requestModel.StockQuantity.Value;

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
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted
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
                    .AsNoTracking()
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
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted
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
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    IsDeleted = item.IsDeleted
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
