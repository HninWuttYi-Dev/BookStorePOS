using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Book;
using Microsoft.EntityFrameworkCore;

namespace BookStorePOS.Domain.Features.Book;

public class BookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BookListResponseModel> GetBooksAsync(BookListRequestModel requestModel)
    {
        try
        {
            var lst = await _db.TblBooks
                    .AsNoTracking()
                    .Where(b => !b.IsDeleted)
                    .ToListAsync();
            List<BookModel> books = new List<BookModel>();
            foreach (var item in lst)
            {
                books.Add(new BookModel
                {
                    BookId = item.BookId,
                    Title = item.Title,
                    Author = item.Author,
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    IsDeleted = item.IsDeleted
                });
            }

            return new BookListResponseModel
            {
                isSuccess = true,
                Message = "Books fetched successfully",
                Data = books
            };
        }
        catch (Exception ex)
        {
            return new BookListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch books: " + ex.Message
            };
        }
    }

    public async Task<BookByIdResponseModel> GetBookAsync(BookByIdRequestModel requestModel)
    {
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
                return new BookByIdResponseModel
                {
                    isSuccess = false,
                    Message = "Book is not found"
                };
            }
            return new BookByIdResponseModel
            {
                isSuccess = true,
                Message = "Book fetched successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Title = item.Title,
                    Author = item.Author,
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            return new BookByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch book: " + ex.Message
            };
        }
    }

    public async Task<BookCreateResponseModel> CreateBookAsync(BookCreateRequestModel requestModel)
    {
        try
        {
            if (requestModel.Price <= 0)
            {
                return new BookCreateResponseModel
                {
                    isSuccess = false,
                    Message = "Price must be greater than 0."
                };
            }

            TblBook book = new TblBook
            {
                Title = requestModel.Title,
                Author = requestModel.Author,
                Genre = requestModel.Genre,
                Description = requestModel.Description,
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBooks.Add(book);
            await _db.SaveChangesAsync();

            return new BookCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new book successfully",
                Data = new BookModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    Description = book.Description,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    IsDeleted = book.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            return new BookCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create book: " + ex.Message
            };
        }
    }

    public async Task<BookPatchResponseModel> UpdateBookAsync(BookPatchRequestModel requestModel)
    {
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

                return new BookPatchResponseModel
                {
                    isSuccess = false,
                    Message = "Book doesn't exist"
                };
            }

            if (!string.IsNullOrEmpty(requestModel.Title)) item.Title = requestModel.Title;
            if (!string.IsNullOrEmpty(requestModel.Author)) item.Author = requestModel.Author;
            if (!string.IsNullOrEmpty(requestModel.Genre)) item.Genre = requestModel.Genre;
            if (requestModel.Description != null) item.Description = requestModel.Description;
            if (requestModel.Price.HasValue) item.Price = requestModel.Price.Value;
            if (requestModel.StockQuantity.HasValue) item.StockQuantity = requestModel.StockQuantity.Value;

            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new BookPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated book successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Title = item.Title,
                    Author = item.Author,
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            return new BookPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update book: " + ex.Message
            };
        }
    }

    public async Task<BookDeleteResponseModel> DeleteBookAsync(BookDeleteRequestModel requestModel)
    {
        try
        {
            var item = await _db.TblBooks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                    x.BookId == requestModel.BookId);
            if (item is null)
            {
                return new BookDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Book is not found"
                };
            }

            // Soft delete
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new BookDeleteResponseModel
            {
                isSuccess = true,
                Message = "Book is deleted successfully",
                Data = new BookModel
                {
                    BookId = item.BookId,
                    Title = item.Title,
                    Author = item.Author,
                    Genre = item.Genre,
                    Description = item.Description,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    IsDeleted = item.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            return new BookDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete book: " + ex.Message
            };
        }
    }
}
