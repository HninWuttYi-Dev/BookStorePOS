using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Book;
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
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.TblBookEditions)
                        .ThenInclude(e => e.Edition)
                    .AsNoTracking()
                    .Where(b => !b.IsDeleted);

            //apply filters if the user sent a value
            if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
            {
                var queryTerm = requestModel.SearchQuery.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(queryTerm) ||
                                         (b.Author != null && b.Author.AuthorName.ToLower().Contains(queryTerm)) ||
                                         b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.ToLower().Contains(queryTerm)));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                query = query.Where(b => b.Title.Contains(requestModel.Title));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                query = query.Where(b => b.Author != null && b.Author.AuthorName.Contains(requestModel.Author));
            }
            if (requestModel.AuthorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == requestModel.AuthorId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                query = query.Where(b => b.Genre != null && b.Genre.GenreName.Contains(requestModel.Genre));
            }
            if (requestModel.GenreId.HasValue)
            {
                query = query.Where(b => b.GenreId == requestModel.GenreId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                query = query.Where(b => b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.Contains(requestModel.Isbn)));
            }

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
                    Title = item.Title,
                    Author = item.Author?.AuthorName,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre?.GenreName,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    IsDeleted = item.IsDeleted,
                    StartingPrice = item.TblBookEditions.Any(e => !e.IsDeleted) ? item.TblBookEditions.Where(e => !e.IsDeleted).Min(e => e.Price) : 0,
                    TotalStockQuantity = item.TblBookEditions.Where(e => !e.IsDeleted).Sum(e => e.StockQuantity),
                    Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel {
                        BookEditionId = e.BookEditionId,
                        BookId = e.BookId,
                        EditionId = e.EditionId,
                        EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                        Isbn = e.Isbn,
                        Price = e.Price,
                        StockQuantity = e.StockQuantity,
                        ReorderLevel = e.ReorderLevel,
                        CoverImageUrl = e.CoverImageUrl,
                        IsDeleted = e.IsDeleted
                    }).ToList()
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
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .Include(b => b.TblBookEditions)
                            .ThenInclude(e => e.Edition)
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
                    Title = item.Title,
                    Author = item.Author?.AuthorName,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre?.GenreName,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    IsDeleted = item.IsDeleted,
                    StartingPrice = item.TblBookEditions.Any(e => !e.IsDeleted) ? item.TblBookEditions.Where(e => !e.IsDeleted).Min(e => e.Price) : 0,
                    TotalStockQuantity = item.TblBookEditions.Where(e => !e.IsDeleted).Sum(e => e.StockQuantity),
                    Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel {
                        BookEditionId = e.BookEditionId,
                        BookId = e.BookId,
                        EditionId = e.EditionId,
                        EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                        Isbn = e.Isbn,
                        Price = e.Price,
                        StockQuantity = e.StockQuantity,
                        ReorderLevel = e.ReorderLevel,
                        CoverImageUrl = e.CoverImageUrl,
                        IsDeleted = e.IsDeleted
                    }).ToList()
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
        using var transaction = await _db.Database.BeginTransactionAsync();
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
                return new BookCreateResponseModel { isSuccess = false, Message = "Price must be greater than 0." };
            }

            // Handle Author
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
                    var newAuthor = new TblAuthor { AuthorName = authorName, CreateAt = DateTime.Now, IsDeleted = false };
                    _db.TblAuthors.Add(newAuthor);
                    await _db.SaveChangesAsync();
                    requestModel.AuthorId = newAuthor.AuthorId;
                }
            }
            else requestModel.AuthorId = null;

            // Handle Genre
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
                    var newGenre = new TblGenre { GenreName = genreName, CreatedAt = DateTime.Now, IsDeleted = false };
                    _db.TblGenres.Add(newGenre);
                    await _db.SaveChangesAsync();
                    requestModel.GenreId = newGenre.GenreId;
                }
            }
            else requestModel.GenreId = null;

            // Handle ISBN
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                    return new BookCreateResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." };

                bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && !e.IsDeleted);
                if (isbnExists)
                    return new BookCreateResponseModel { isSuccess = false, Message = "A book edition with this ISBN already exists." };
            }

            if (requestModel.StockQuantity < 0)
                return new BookCreateResponseModel { isSuccess = false, Message = "StockQuantity cannot be negative." };

            // Create Book
            TblBook book = new TblBook
            {
                Title = requestModel.Title,
                AuthorId = requestModel.AuthorId,
                GenreId = requestModel.GenreId,
                Description = requestModel.Description,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBooks.Add(book);
            await _db.SaveChangesAsync();

            // Create or get Edition Type
            var editionName = string.IsNullOrWhiteSpace(requestModel.EditionName) ? "1st Edition" : requestModel.EditionName;
            var editionType = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName.ToLower() == editionName.ToLower());
            if (editionType == null)
            {
                editionType = new TblEdition { EditionName = editionName, CreatedAt = DateTime.Now, IsDeleted = false };
                _db.TblEditions.Add(editionType);
                await _db.SaveChangesAsync();
            }

            // Create Book Edition
            TblBookEdition bookEdition = new TblBookEdition
            {
                BookId = book.BookId,
                EditionId = editionType.EditionId,
                Isbn = requestModel.Isbn,
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                ReorderLevel = requestModel.ReorderLevel,
                CoverImageUrl = requestModel.CoverImageUrl,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBookEditions.Add(bookEdition);
            await _db.SaveChangesAsync();
            
            await transaction.CommitAsync();

            _logger.LogInformation("Create Book Async => Book is created successfully");
            return new BookCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new book successfully",
                Data = new BookModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = authorName,
                    AuthorId = book.AuthorId,
                    Genre = genreName,
                    GenreId = book.GenreId,
                    Description = book.Description,
                    IsDeleted = book.IsDeleted,
                    StartingPrice = bookEdition.Price,
                    TotalStockQuantity = bookEdition.StockQuantity,
                    Editions = new List<BookEditionModel> {
                        new BookEditionModel {
                            BookEditionId = bookEdition.BookEditionId,
                            BookId = bookEdition.BookId,
                            EditionId = bookEdition.EditionId,
                            EditionName = editionType.EditionName,
                            Isbn = bookEdition.Isbn,
                            Price = bookEdition.Price,
                            StockQuantity = bookEdition.StockQuantity,
                            ReorderLevel = bookEdition.ReorderLevel,
                            CoverImageUrl = bookEdition.CoverImageUrl,
                            IsDeleted = bookEdition.IsDeleted
                        }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .FirstOrDefaultAsync(x => x.BookId == requestModel.BookId && !x.IsDeleted);
            if (item is null)
            {
                _logger.LogWarning("Update Book Async => Book doesn't exist");
                return new BookPatchResponseModel { isSuccess = false, Message = "Book doesn't exist" };
            }

            if (requestModel.Title != null && string.IsNullOrWhiteSpace(requestModel.Title))
                return new BookPatchResponseModel { isSuccess = false, Message = "Title cannot be empty." };

            // Handle Author
            string authorName = item.Author?.AuthorName ?? "";
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
                    var newAuthor = new TblAuthor { AuthorName = requestModel.Author, CreateAt = DateTime.Now, IsDeleted = false };
                    _db.TblAuthors.Add(newAuthor);
                    await _db.SaveChangesAsync();
                    requestModel.AuthorId = newAuthor.AuthorId;
                    authorName = newAuthor.AuthorName;
                }
            }
            else if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value <= 0) requestModel.AuthorId = null;

            // Handle Genre
            string genreName = item.Genre?.GenreName ?? "";
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
                    var newGenre = new TblGenre { GenreName = requestModel.Genre, CreatedAt = DateTime.Now, IsDeleted = false };
                    _db.TblGenres.Add(newGenre);
                    await _db.SaveChangesAsync();
                    requestModel.GenreId = newGenre.GenreId;
                    genreName = newGenre.GenreName;
                }
            }
            else if (requestModel.GenreId.HasValue && requestModel.GenreId.Value <= 0) requestModel.GenreId = null;

            if (!string.IsNullOrEmpty(requestModel.Title)) item.Title = requestModel.Title;
            
            if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value > 0) item.AuthorId = requestModel.AuthorId;

            if (requestModel.GenreId.HasValue && requestModel.GenreId.Value > 0) item.GenreId = requestModel.GenreId;

            if (requestModel.Description != null) item.Description = requestModel.Description;


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
                    Title = item.Title,
                    Author = authorName,
                    AuthorId = item.AuthorId,
                    Genre = genreName,
                    GenreId = item.GenreId,
                    Description = item.Description,
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
            var item = await _db.TblBooks.FirstOrDefaultAsync(x => x.BookId == requestModel.BookId);
            if (item is null)
                return new BookDeleteResponseModel { isSuccess = false, Message = "Book is not found" };

            if (item.IsDeleted)
                return new BookDeleteResponseModel { isSuccess = false, Message = "Book is already deleted." };

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            
            // Delete editions as well
            var editions = await _db.TblBookEditions.Where(e => e.BookId == item.BookId).ToListAsync();
            foreach (var edition in editions)
            {
                edition.IsDeleted = true;
                edition.UpdatedAt = DateTime.Now;
                _db.Entry(edition).State = EntityState.Modified;
            }
            
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Book Async => Book is deleted successfully");
            return new BookDeleteResponseModel
            {
                isSuccess = true,
                Message = "Book is deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Book Async => Failed to delete book {Message}", ex.Message);
            return new BookDeleteResponseModel { isSuccess = false, Message = "Failed to delete book: " + ex.Message };
        }
    }

    public async Task<BookListResponseModel> GetLowStockBooksAsync()
    {
        _logger.LogInformation("Get Low Stock Books Async => Fetching low stock books");
        try
        {
            // Low stock means ANY edition is low stock
            var query = _db.TblBooks
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.TblBookEditions)
                    .ThenInclude(e => e.Edition)
                .AsNoTracking()
                .Where(b => !b.IsDeleted && b.TblBookEditions.Any(e => !e.IsDeleted && e.StockQuantity <= e.ReorderLevel));

            var lst = await query.ToListAsync();
            List<BookModel> books = new List<BookModel>();
            foreach (var item in lst)
            {
                books.Add(new BookModel
                {
                    BookId = item.BookId,
                    Title = item.Title,
                    Author = item.Author?.AuthorName,
                    AuthorId = item.AuthorId,
                    Genre = item.Genre?.GenreName,
                    GenreId = item.GenreId,
                    Description = item.Description,
                    IsDeleted = item.IsDeleted,
                    StartingPrice = item.TblBookEditions.Any(e => !e.IsDeleted) ? item.TblBookEditions.Where(e => !e.IsDeleted).Min(e => e.Price) : 0,
                    TotalStockQuantity = item.TblBookEditions.Where(e => !e.IsDeleted).Sum(e => e.StockQuantity),
                    Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel {
                        BookEditionId = e.BookEditionId,
                        BookId = e.BookId,
                        EditionId = e.EditionId,
                        EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                        Isbn = e.Isbn,
                        Price = e.Price,
                        StockQuantity = e.StockQuantity,
                        ReorderLevel = e.ReorderLevel,
                        CoverImageUrl = e.CoverImageUrl,
                        IsDeleted = e.IsDeleted
                    }).ToList()
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

    public async Task<BookEditionCreateResponseModel> CreateBookEditionAsync(BookEditionCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Book Edition Async => Creating book edition for BookId {BookId}", requestModel.BookId);
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var book = await _db.TblBooks.FirstOrDefaultAsync(b => b.BookId == requestModel.BookId && !b.IsDeleted);
            if (book == null)
            {
                return new BookEditionCreateResponseModel { isSuccess = false, Message = "Book not found." };
            }

            if (string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                return new BookEditionCreateResponseModel { isSuccess = false, Message = "Edition Name is required." };
            }
            if (requestModel.Price <= 0)
            {
                return new BookEditionCreateResponseModel { isSuccess = false, Message = "Price must be greater than 0." };
            }
            if (requestModel.StockQuantity < 0)
            {
                return new BookEditionCreateResponseModel { isSuccess = false, Message = "StockQuantity cannot be negative." };
            }

            // Validate duplicate edition name for this book
            var existingEditionNameCheck = await _db.TblBookEditions
                .Include(be => be.Edition)
                .FirstOrDefaultAsync(be => be.BookId == requestModel.BookId && be.Edition != null && be.Edition.EditionName.ToLower() == requestModel.EditionName.ToLower() && !be.IsDeleted);

            if (existingEditionNameCheck != null)
            {
                return new BookEditionCreateResponseModel { isSuccess = false, Message = "This book already has an edition with this name." };
            }

            // Handle ISBN
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                    return new BookEditionCreateResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." };

                bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && !e.IsDeleted);
                if (isbnExists)
                    return new BookEditionCreateResponseModel { isSuccess = false, Message = "A book edition with this ISBN already exists." };
            }

            // Create or get Edition Type
            var editionName = requestModel.EditionName;
            var editionType = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName.ToLower() == editionName.ToLower());
            if (editionType == null)
            {
                editionType = new TblEdition { EditionName = editionName, CreatedAt = DateTime.Now, IsDeleted = false };
                _db.TblEditions.Add(editionType);
                await _db.SaveChangesAsync();
            }

            // Create Book Edition
            TblBookEdition bookEdition = new TblBookEdition
            {
                BookId = requestModel.BookId,
                EditionId = editionType.EditionId,
                Isbn = requestModel.Isbn,
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                ReorderLevel = requestModel.ReorderLevel,
                CoverImageUrl = requestModel.CoverImageUrl,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBookEditions.Add(bookEdition);
            await _db.SaveChangesAsync();
            
            await transaction.CommitAsync();

            _logger.LogInformation("Create Book Edition Async => Book Edition is created successfully");
            return new BookEditionCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new book edition successfully",
                Data = new BookEditionModel
                {
                    BookEditionId = bookEdition.BookEditionId,
                    BookId = bookEdition.BookId,
                    EditionId = bookEdition.EditionId,
                    EditionName = editionType.EditionName,
                    Isbn = bookEdition.Isbn,
                    Price = bookEdition.Price,
                    StockQuantity = bookEdition.StockQuantity,
                    ReorderLevel = bookEdition.ReorderLevel,
                    CoverImageUrl = bookEdition.CoverImageUrl,
                    IsDeleted = bookEdition.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning("Create Book Edition Async => Failed to create book edition {Message}", ex.Message);
            return new BookEditionCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create book edition: " + ex.Message
            };
        }
    }

    public async Task<BookEditionPatchResponseModel> UpdateBookEditionAsync(BookEditionPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Book Edition Async => Updating book edition");
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var bookEdition = await _db.TblBookEditions
                .Include(be => be.Edition)
                .FirstOrDefaultAsync(x => x.BookEditionId == requestModel.BookEditionId && !x.IsDeleted);

            if (bookEdition == null)
            {
                return new BookEditionPatchResponseModel { isSuccess = false, Message = "Book edition not found." };
            }

            if (requestModel.Price <= 0)
            {
                return new BookEditionPatchResponseModel { isSuccess = false, Message = "Price must be greater than 0." };
            }
            if (requestModel.StockQuantity < 0)
            {
                return new BookEditionPatchResponseModel { isSuccess = false, Message = "StockQuantity cannot be negative." };
            }

            // Handle ISBN uniqueness if changed
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn) && requestModel.Isbn != bookEdition.Isbn)
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                    return new BookEditionPatchResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." };

                bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && e.BookEditionId != requestModel.BookEditionId && !e.IsDeleted);
                if (isbnExists)
                    return new BookEditionPatchResponseModel { isSuccess = false, Message = "A book edition with this ISBN already exists." };
            }

            // Handle Edition Name change
            if (!string.IsNullOrWhiteSpace(requestModel.EditionName) && bookEdition.Edition?.EditionName != requestModel.EditionName)
            {
                // Validate duplicate edition name for this book (excluding current edition)
                var existingEditionNameCheck = await _db.TblBookEditions
                    .Include(be => be.Edition)
                    .FirstOrDefaultAsync(be => be.BookId == bookEdition.BookId && be.BookEditionId != bookEdition.BookEditionId && be.Edition != null && be.Edition.EditionName.ToLower() == requestModel.EditionName.ToLower() && !be.IsDeleted);

                if (existingEditionNameCheck != null)
                {
                    return new BookEditionPatchResponseModel { isSuccess = false, Message = "This book already has an edition with this name." };
                }

                var editionType = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName.ToLower() == requestModel.EditionName.ToLower());
                if (editionType == null)
                {
                    editionType = new TblEdition { EditionName = requestModel.EditionName, CreatedAt = DateTime.Now, IsDeleted = false };
                    _db.TblEditions.Add(editionType);
                    await _db.SaveChangesAsync();
                }
                bookEdition.EditionId = editionType.EditionId;
            }

            bookEdition.Price = requestModel.Price;
            bookEdition.StockQuantity = requestModel.StockQuantity;
            bookEdition.ReorderLevel = requestModel.ReorderLevel;
            bookEdition.Isbn = requestModel.Isbn;
            if (requestModel.CoverImageUrl != null)
            {
                bookEdition.CoverImageUrl = requestModel.CoverImageUrl;
            }
            if (!_db.ChangeTracker.HasChanges())
            {
                await transaction.CommitAsync();
                return new BookEditionPatchResponseModel
                {
                    isSuccess = true,
                    Message = "No changes detected.",
                    Data = new BookEditionModel
                    {
                        BookEditionId = bookEdition.BookEditionId,
                        BookId = bookEdition.BookId,
                        EditionId = bookEdition.EditionId,
                        EditionName = bookEdition.Edition?.EditionName ?? "",
                        Isbn = bookEdition.Isbn,
                        Price = bookEdition.Price,
                        StockQuantity = bookEdition.StockQuantity,
                        ReorderLevel = bookEdition.ReorderLevel,
                        CoverImageUrl = bookEdition.CoverImageUrl,
                        IsDeleted = bookEdition.IsDeleted
                    }
                };
            }

            bookEdition.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Update Book Edition Async => Book Edition updated successfully");
            return new BookEditionPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated book edition successfully",
                Data = new BookEditionModel
                {
                    BookEditionId = bookEdition.BookEditionId,
                    BookId = bookEdition.BookId,
                    EditionId = bookEdition.EditionId,
                    EditionName = requestModel.EditionName,
                    Isbn = bookEdition.Isbn,
                    Price = bookEdition.Price,
                    StockQuantity = bookEdition.StockQuantity,
                    ReorderLevel = bookEdition.ReorderLevel,
                    CoverImageUrl = bookEdition.CoverImageUrl,
                    IsDeleted = bookEdition.IsDeleted
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning("Update Book Edition Async => Failed to update book edition {Message}", ex.Message);
            return new BookEditionPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update book edition: " + ex.Message
            };
        }
    }

    public async Task<BookEditionDeleteResponseModel> DeleteBookEditionAsync(BookEditionDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Book Edition Async => Deleting book edition");
        try
        {
            var bookEdition = await _db.TblBookEditions.FirstOrDefaultAsync(x => x.BookEditionId == requestModel.BookEditionId);
            if (bookEdition == null)
            {
                return new BookEditionDeleteResponseModel { isSuccess = false, Message = "Book edition not found." };
            }

            if (bookEdition.IsDeleted)
            {
                return new BookEditionDeleteResponseModel { isSuccess = false, Message = "Book edition is already deleted." };
            }

            bookEdition.IsDeleted = true;
            bookEdition.UpdatedAt = DateTime.Now;
            _db.Entry(bookEdition).State = EntityState.Modified;
            
            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Book Edition Async => Book Edition deleted successfully");
            return new BookEditionDeleteResponseModel
            {
                isSuccess = true,
                Message = "Book edition deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Book Edition Async => Failed to delete book edition {Message}", ex.Message);
            return new BookEditionDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete book edition: " + ex.Message
            };
        }
    }
}
