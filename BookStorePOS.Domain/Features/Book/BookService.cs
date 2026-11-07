using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Book;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Order;
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
            var baseQuery = _db.TblBooks
                .AsNoTracking()
                .Where(b => !b.IsDeleted);

            // Apply filters if the user sent a value
            if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
            {
                var queryTerm = requestModel.SearchQuery.Trim().ToLower();
                baseQuery = baseQuery.Where(b => b.Title.ToLower().Contains(queryTerm) ||
                                                 (b.Author != null && b.Author.AuthorName.ToLower().Contains(queryTerm)) ||
                                                 b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.ToLower().Contains(queryTerm)));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                baseQuery = baseQuery.Where(b => b.Title.Contains(requestModel.Title));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                baseQuery = baseQuery.Where(b => b.Author != null && b.Author.AuthorName.Contains(requestModel.Author));
            }
            if (requestModel.AuthorId.HasValue)
            {
                baseQuery = baseQuery.Where(b => b.AuthorId == requestModel.AuthorId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                baseQuery = baseQuery.Where(b => b.Genre != null && b.Genre.GenreName.Contains(requestModel.Genre));
            }
            if (requestModel.GenreId.HasValue)
            {
                baseQuery = baseQuery.Where(b => b.GenreId == requestModel.GenreId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                baseQuery = baseQuery.Where(b => b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.Contains(requestModel.Isbn)));
            }
            if (requestModel.OnlyLowStock == true)
            {
                baseQuery = baseQuery.Where(b => b.TblBookEditions.Any(e => !e.IsDeleted && e.StockQuantity <= e.ReorderLevel));
            }

            var totalCount = await baseQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)requestModel.Limit);
            if (totalPages == 0) totalPages = 1;

            var lst = await baseQuery
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.TblBookEditions)
                    .ThenInclude(e => e.Edition)
                .OrderByDescending(b => b.BookId)
                .Skip((requestModel.Page - 1) * requestModel.Limit)
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
                    Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel
                    {
                        BookEditionId = e.BookEditionId,
                        BookId = e.BookId,
                        EditionId = e.EditionId,
                        EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                        Isbn = e.Isbn,
                        Price = e.Price,
                        StockQuantity = e.StockQuantity,
                        EditionNote = e.EditionNote,
                        PublishDate = e.PublishDate,
                        PageCount = e.PageCount,
                        ReorderLevel = e.ReorderLevel,
                        CoverImageUrl = e.CoverImageUrl,
                        Publisher = e.Publisher,
                        IsDeleted = e.IsDeleted
                    }).ToList()
                });
            }
            _logger.LogInformation("Get Books Async => Books fetched successfully");
            return new BookListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Books fetched successfully",
                Data = books,
                Page = requestModel.Page,
                Limit = requestModel.Limit,
                Count = totalCount,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Books Async => Failed to fetch books {Message}", ex.Message);
            return new BookListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch books: " + ex.Message
            };
        }
    }

    public async Task<BookCursorResponseModel> GetBooksCursorAsync(BookCursorRequestModel requestModel)
    {
        _logger.LogInformation("Get Books Cursor Async => Cursor: {Cursor}, Limit: {Limit}", requestModel.Cursor, requestModel.Limit);
        try
        {
            var baseQuery = _db.TblBooks
                .AsNoTracking()
                .Where(b => !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(requestModel.SearchQuery))
            {
                var queryTerm = requestModel.SearchQuery.Trim().ToLower();
                baseQuery = baseQuery.Where(b => b.Title.ToLower().Contains(queryTerm) ||
                                                 (b.Author != null && b.Author.AuthorName.ToLower().Contains(queryTerm)) ||
                                                 b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.ToLower().Contains(queryTerm)));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Title))
            {
                baseQuery = baseQuery.Where(b => b.Title.Contains(requestModel.Title));
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                baseQuery = baseQuery.Where(b => b.Author != null && b.Author.AuthorName.Contains(requestModel.Author));
            }
            if (requestModel.AuthorId.HasValue)
            {
                baseQuery = baseQuery.Where(b => b.AuthorId == requestModel.AuthorId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                baseQuery = baseQuery.Where(b => b.Genre != null && b.Genre.GenreName.Contains(requestModel.Genre));
            }
            if (requestModel.GenreId.HasValue)
            {
                baseQuery = baseQuery.Where(b => b.GenreId == requestModel.GenreId);
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                baseQuery = baseQuery.Where(b => b.TblBookEditions.Any(e => e.Isbn != null && e.Isbn.Contains(requestModel.Isbn)));
            }
            if (requestModel.OnlyLowStock == true)
            {
                baseQuery = baseQuery.Where(b => b.TblBookEditions.Any(e => !e.IsDeleted && e.StockQuantity <= e.ReorderLevel));
            }

            var totalCount = await baseQuery.CountAsync();

            // Deterministic Cursor filter (O(1) index seek on primary key)
            if (requestModel.Cursor.HasValue && requestModel.Cursor.Value > 0)
            {
                baseQuery = baseQuery.Where(b => b.BookId < requestModel.Cursor.Value);
            }

            int takeCount = requestModel.Limit <= 0 ? 12 : requestModel.Limit;

            // Take takeCount + 1 to check if there is a next page without a second count query
            var rawList = await baseQuery
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.TblBookEditions)
                    .ThenInclude(e => e.Edition)
                .OrderByDescending(b => b.BookId)
                .Take(takeCount + 1)
                .ToListAsync();

            bool hasNextPage = rawList.Count > takeCount;
            if (hasNextPage)
            {
                rawList.RemoveAt(rawList.Count - 1);
            }

            int? nextCursor = hasNextPage && rawList.Any() ? rawList.Last().BookId : null;

            var books = rawList.Select(item => new BookModel
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
                Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel
                {
                    BookEditionId = e.BookEditionId,
                    BookId = e.BookId,
                    EditionId = e.EditionId,
                    EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                    Isbn = e.Isbn,
                    Price = e.Price,
                    StockQuantity = e.StockQuantity,
                    ReorderLevel = e.ReorderLevel,
                    PublishDate = e.PublishDate,
                    PageCount = e.PageCount,
                    Publisher = e.Publisher,
                    CoverImageUrl = e.CoverImageUrl,
                    EditionNote = e.EditionNote
                }).ToList()
            }).ToList();

            return new BookCursorResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Books fetched successfully via cursor",
                Data = books,
                NextCursor = nextCursor,
                HasNextPage = hasNextPage,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBooksCursorAsync => Exception occurred");
            return new BookCursorResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch books via cursor: " + ex.Message
            };
        }
    }

    public async Task<Result<BookModel>> GetBookAsync(BookByIdRequestModel requestModel)
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
                return Result<BookModel>.NotFound("Book is not found");
            }

            _logger.LogInformation("Get Book ById Async => Book fetched successfully");
            return Result<BookModel>.Success(new BookModel
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
                Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel
                {
                    BookEditionId = e.BookEditionId,
                    BookId = e.BookId,
                    EditionId = e.EditionId,
                    EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                    Isbn = e.Isbn,
                    Price = e.Price,
                    StockQuantity = e.StockQuantity,
                    EditionNote = e.EditionNote,
                    PublishDate = e.PublishDate,
                    PageCount = e.PageCount,
                    ReorderLevel = e.ReorderLevel,
                    CoverImageUrl = e.CoverImageUrl,
                    Publisher = e.Publisher,
                    IsDeleted = e.IsDeleted
                }).ToList()
            }, "Book fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Book ById Async => Failed to fetch book {Message}", ex.Message);
            return Result<BookModel>.SystemError("Failed to fetch book: " + ex.Message);
        }
    }

    public async Task<Result<BookModel>> CreateBookAsync(BookCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Book Async => Creating book");

        // 1. Pure in-memory validation
        if (string.IsNullOrWhiteSpace(requestModel.Title))
        {
            _logger.LogWarning("Create Book Async => Title is required");
            return Result<BookModel>.ValidationError("Title is required.");
        }
        if ((!requestModel.AuthorId.HasValue || requestModel.AuthorId.Value <= 0) && string.IsNullOrWhiteSpace(requestModel.Author))
        {
            _logger.LogWarning("Create Book Async => Author is required");
            return Result<BookModel>.ValidationError("Author is required.");
        }
        if ((!requestModel.GenreId.HasValue || requestModel.GenreId.Value <= 0) && string.IsNullOrWhiteSpace(requestModel.Genre))
        {
            _logger.LogWarning("Create Book Async => Genre is required");
            return Result<BookModel>.ValidationError("Genre is required.");
        }
        if (requestModel.Price <= 0)
        {
            _logger.LogWarning("Create Book Async => Price must be greater than 0");
            return Result<BookModel>.ValidationError("Price must be greater than 0.");
        }
        if (requestModel.StockQuantity < 0)
        {
            _logger.LogWarning("Create Book Async => StockQuantity cannot be negative");
            return Result<BookModel>.ValidationError("StockQuantity cannot be negative.");
        }
        if (requestModel.ReorderLevel < 0)
        {
            _logger.LogWarning("Create Book Async => ReorderLevel cannot be negative");
            return Result<BookModel>.ValidationError("ReorderLevel cannot be negative.");
        }

        if (requestModel.PublishDate.HasValue && requestModel.PublishDate.Value.Date > DateTime.Today)
        {
            _logger.LogWarning("Create Book Async => Publication Date cannot be in the future");
            return Result<BookModel>.ValidationError("Publication Date cannot be in the future.");
        }

        // ISBN validation
        if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
        {
            var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
            if (isbn.Length != 10 && isbn.Length != 13)
                return Result<BookModel>.ValidationError("ISBN must be 10 or 13 digits.");

            bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && !e.IsDeleted);
            if (isbnExists)
                return Result<BookModel>.Conflict("A book edition with this ISBN already exists.");
        }

        // 2. Resolve Author (in-memory entity or existing tracked entity)
        string authorName = (requestModel.Author ?? "").Trim();
        TblAuthor? authorEntity = null;
        if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value > 0)
        {
            authorEntity = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorId == requestModel.AuthorId && !a.IsDeleted);
            if (authorEntity == null)
                return Result<BookModel>.ValidationError("Invalid Author.");
            authorName = authorEntity.AuthorName;
        }
        else if (!string.IsNullOrWhiteSpace(authorName))
        {
            authorEntity = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorName.ToLower() == authorName.ToLower() && !a.IsDeleted);
            if (authorEntity == null)
            {
                authorEntity = new TblAuthor { AuthorName = authorName, CreateAt = DateTime.Now, IsDeleted = false };
                _db.TblAuthors.Add(authorEntity);
            }
            else
            {
                authorName = authorEntity.AuthorName;
            }
        }

        // 3. Resolve Genre (in-memory entity or existing tracked entity)
        string genreName = (requestModel.Genre ?? "").Trim();
        TblGenre? genreEntity = null;
        if (requestModel.GenreId.HasValue && requestModel.GenreId.Value > 0)
        {
            genreEntity = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreId == requestModel.GenreId && !g.IsDeleted);
            if (genreEntity == null)
                return Result<BookModel>.ValidationError("Invalid Genre.");
            genreName = genreEntity.GenreName;
        }
        else if (!string.IsNullOrWhiteSpace(genreName))
        {
            genreEntity = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreName.ToLower() == genreName.ToLower() && !g.IsDeleted);
            if (genreEntity == null)
            {
                genreEntity = new TblGenre { GenreName = genreName, CreatedAt = DateTime.Now, IsDeleted = false };
                _db.TblGenres.Add(genreEntity);
            }
            else
            {
                genreName = genreEntity.GenreName;
            }
        }

        // 4. Resolve Edition Type
        var editionName = string.IsNullOrWhiteSpace(requestModel.EditionName) ? "1st Edition" : requestModel.EditionName.Trim();
        var editionType = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName.ToLower() == editionName.ToLower() && !e.IsDeleted);
        if (editionType == null)
        {
            editionType = new TblEdition { EditionName = editionName, CreatedAt = DateTime.Now, IsDeleted = false };
            _db.TblEditions.Add(editionType);
        }

        // 5. Build entity navigation graph in memory
        var book = new TblBook
        {
            Title = requestModel.Title.Trim(),
            Author = authorEntity,
            Genre = genreEntity,
            AuthorId = authorEntity?.AuthorId,
            GenreId = genreEntity?.GenreId,
            Description = requestModel.Description,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        var bookEdition = new TblBookEdition
        {
            Book = book,
            Edition = editionType,
            EditionId = editionType.EditionId,
            Isbn = string.IsNullOrWhiteSpace(requestModel.Isbn) ? null : requestModel.Isbn.Trim(),
            Price = requestModel.Price,
            StockQuantity = requestModel.StockQuantity,
            ReorderLevel = requestModel.ReorderLevel,
            EditionNote = requestModel.EditionNote,
            PublishDate = requestModel.PublishDate ?? DateTime.Now,
            PageCount = requestModel.PageCount,
            CoverImageUrl = requestModel.CoverImageUrl,
            Publisher = requestModel.Publisher,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        book.TblBookEditions.Add(bookEdition);
        _db.TblBooks.Add(book);

        // 6. Single atomic SaveChangesAsync and transaction commit
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Create Book Async => Book is created successfully");
            return Result<BookModel>.Success(new BookModel
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
                Editions = new List<BookEditionModel>
                {
                    new BookEditionModel
                    {
                        BookEditionId = bookEdition.BookEditionId,
                        BookId = bookEdition.BookId,
                        EditionId = bookEdition.EditionId,
                        EditionName = editionType.EditionName,
                        Isbn = bookEdition.Isbn,
                        Price = bookEdition.Price,
                        StockQuantity = bookEdition.StockQuantity,
                        EditionNote = bookEdition.EditionNote,
                        PublishDate = bookEdition.PublishDate,
                        PageCount = bookEdition.PageCount,
                        ReorderLevel = bookEdition.ReorderLevel,
                        CoverImageUrl = bookEdition.CoverImageUrl,
                        Publisher = bookEdition.Publisher,
                        IsDeleted = bookEdition.IsDeleted
                    }
                }
            }, "Created new book successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning("Create Book Async => Failed to create book {Message}", ex?.InnerException?.Message ?? ex?.Message);
            return Result<BookModel>.SystemError("Failed to create book: " + (ex?.InnerException?.Message ?? ex?.Message));
        }
    }

    public async Task<Result<BookModel>> UpdateBookAsync(BookPatchRequestModel requestModel)
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
                return Result<BookModel>.NotFound("Book doesn't exist");
            }

            if (requestModel.Title != null && string.IsNullOrWhiteSpace(requestModel.Title))
                return Result<BookModel>.ValidationError("Title cannot be empty.");

            // Handle Author
            string authorName = item.Author?.AuthorName ?? "";
            TblAuthor? targetAuthor = null;
            if (requestModel.AuthorId.HasValue && requestModel.AuthorId.Value > 0)
            {
                var authorEntity = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorId == requestModel.AuthorId && !a.IsDeleted);
                // If user changed the name manually and it differs from the entity with this ID, re-resolve by name
                if (authorEntity != null && !string.IsNullOrWhiteSpace(requestModel.Author) && !string.Equals(authorEntity.AuthorName, requestModel.Author.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    targetAuthor = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorName.ToLower() == requestModel.Author.Trim().ToLower() && !a.IsDeleted);
                    if (targetAuthor == null)
                    {
                        targetAuthor = new TblAuthor { AuthorName = requestModel.Author.Trim(), CreateAt = DateTime.Now, IsDeleted = false };
                        _db.TblAuthors.Add(targetAuthor);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    targetAuthor = authorEntity;
                }
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.Author))
            {
                targetAuthor = await _db.TblAuthors.FirstOrDefaultAsync(a => a.AuthorName.ToLower() == requestModel.Author.Trim().ToLower() && !a.IsDeleted);
                if (targetAuthor == null)
                {
                    targetAuthor = new TblAuthor { AuthorName = requestModel.Author.Trim(), CreateAt = DateTime.Now, IsDeleted = false };
                    _db.TblAuthors.Add(targetAuthor);
                    await _db.SaveChangesAsync();
                }
            }

            if (targetAuthor != null)
            {
                item.Author = targetAuthor;
                item.AuthorId = targetAuthor.AuthorId;
                authorName = targetAuthor.AuthorName;
            }

            // Handle Genre
            string genreName = item.Genre?.GenreName ?? "";
            TblGenre? targetGenre = null;
            if (requestModel.GenreId.HasValue && requestModel.GenreId.Value > 0)
            {
                var genreEntity = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreId == requestModel.GenreId && !g.IsDeleted);
                // If user changed the name manually and it differs from the entity with this ID, re-resolve by name
                if (genreEntity != null && !string.IsNullOrWhiteSpace(requestModel.Genre) && !string.Equals(genreEntity.GenreName, requestModel.Genre.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    targetGenre = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreName.ToLower() == requestModel.Genre.Trim().ToLower() && !g.IsDeleted);
                    if (targetGenre == null)
                    {
                        targetGenre = new TblGenre { GenreName = requestModel.Genre.Trim(), CreatedAt = DateTime.Now, IsDeleted = false };
                        _db.TblGenres.Add(targetGenre);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    targetGenre = genreEntity;
                }
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.Genre))
            {
                targetGenre = await _db.TblGenres.FirstOrDefaultAsync(g => g.GenreName.ToLower() == requestModel.Genre.Trim().ToLower() && !g.IsDeleted);
                if (targetGenre == null)
                {
                    targetGenre = new TblGenre { GenreName = requestModel.Genre.Trim(), CreatedAt = DateTime.Now, IsDeleted = false };
                    _db.TblGenres.Add(targetGenre);
                    await _db.SaveChangesAsync();
                }
            }

            if (targetGenre != null)
            {
                item.Genre = targetGenre;
                item.GenreId = targetGenre.GenreId;
                genreName = targetGenre.GenreName;
            }

            if (!string.IsNullOrEmpty(requestModel.Title)) item.Title = requestModel.Title;
            if (requestModel.Description != null) item.Description = requestModel.Description;

            item.UpdatedAt = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Update Book Async => Book is updated successfully");
            return Result<BookModel>.Success(new BookModel
            {
                BookId = item.BookId,
                Title = item.Title,
                Author = authorName,
                AuthorId = item.AuthorId,
                Genre = genreName,
                GenreId = item.GenreId,
                Description = item.Description,
                IsDeleted = item.IsDeleted
            }, "Updated book successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Update Book Async => Failed to update book {Message}", ex.Message);
            return Result<BookModel>.SystemError("Failed to update book: " + ex.Message);
        }
    }

    public async Task<Result<BookModel>> DeleteBookAsync(BookDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Book Async => Deleting book");
        try
        {
            var item = await _db.TblBooks.FirstOrDefaultAsync(x => x.BookId == requestModel.BookId);
            if (item is null)
                return Result<BookModel>.NotFound("Book is not found");

            if (item.IsDeleted)
                return Result<BookModel>.Conflict("Book is already deleted.");

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
            return Result<BookModel>.DeleteSuccess("Book is deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Book Async => Failed to delete book {Message}", ex.Message);
            return Result<BookModel>.SystemError("Failed to delete book: " + ex.Message);
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
                    Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel
                    {
                        BookEditionId = e.BookEditionId,
                        BookId = e.BookId,
                        EditionId = e.EditionId,
                        EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                        Isbn = e.Isbn,
                        Price = e.Price,
                        StockQuantity = e.StockQuantity,
                        EditionNote = e.EditionNote,
                        PublishDate = e.PublishDate,
                        PageCount = e.PageCount,
                        ReorderLevel = e.ReorderLevel,
                        CoverImageUrl = e.CoverImageUrl,
                        Publisher = e.Publisher,
                        IsDeleted = e.IsDeleted
                    }).ToList()
                });
            }

            _logger.LogInformation("Get Low Stock Books Async => Low stock books fetched successfully");
            return new BookListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Low stock books fetched successfully",
                Data = books
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get Low Stock Books Async => Failed to fetch low stock books");
            return new BookListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = "Failed to fetch low stock books: " + ex.Message
            };
        }
    }

    public async Task<Result<BookEditionModel>> CreateBookEditionAsync(BookEditionCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Book Edition Async => Creating book edition for BookId {BookId}", requestModel.BookId);
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var book = await _db.TblBooks.FirstOrDefaultAsync(b => b.BookId == requestModel.BookId && !b.IsDeleted);
            if (book == null)
            {
                return Result<BookEditionModel>.NotFound("Book not found.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                return Result<BookEditionModel>.ValidationError("Edition Name is required.");
            }
            if (requestModel.Price <= 0)
            {
                return Result<BookEditionModel>.ValidationError("Price must be greater than 0.");
            }
            if (requestModel.StockQuantity < 0)
            {
                return Result<BookEditionModel>.ValidationError("StockQuantity cannot be negative.");
            }

            // Validate duplicate edition name for this book
            var existingEditionNameCheck = await _db.TblBookEditions
                .Include(be => be.Edition)
                .FirstOrDefaultAsync(be => be.BookId == requestModel.BookId && be.Edition != null && be.Edition.EditionName.ToLower() == requestModel.EditionName.ToLower() && !be.IsDeleted);

            if (existingEditionNameCheck != null)
            {
                return Result<BookEditionModel>.Conflict("This book already has an edition with this name.");
            }

            // Handle ISBN
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                    return Result<BookEditionModel>.ValidationError("ISBN must be 10 or 13 digits.");

                bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && !e.IsDeleted);
                if (isbnExists)
                    return Result<BookEditionModel>.Conflict("A book edition with this ISBN already exists.");
            }

            if (requestModel.PublishDate.HasValue && requestModel.PublishDate.Value.Date > DateTime.Today)
            {
                _logger.LogWarning("Create Edition Async => Publication Date cannot be in the future");
                return Result<BookEditionModel>.ValidationError("Publication Date cannot be in the future.");
            }

            // Create or get Edition Type
            var editionName = requestModel.EditionName.Trim();
            var editionType = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName.ToLower() == editionName.ToLower() && !e.IsDeleted);
            if (editionType == null)
            {
                editionType = new TblEdition { EditionName = editionName, CreatedAt = DateTime.Now, IsDeleted = false };
                _db.TblEditions.Add(editionType);
            }

            // Create Book Edition linked to editionType
            TblBookEdition bookEdition = new TblBookEdition
            {
                BookId = requestModel.BookId,
                Edition = editionType,
                EditionId = editionType.EditionId,
                Isbn = string.IsNullOrWhiteSpace(requestModel.Isbn) ? null : requestModel.Isbn.Trim(),
                Price = requestModel.Price,
                StockQuantity = requestModel.StockQuantity,
                ReorderLevel = requestModel.ReorderLevel,
                EditionNote = requestModel.EditionNote,
                PublishDate = requestModel.PublishDate ?? DateTime.Now,
                PageCount = requestModel.PageCount,
                CoverImageUrl = requestModel.CoverImageUrl,
                Publisher = requestModel.Publisher,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            _db.TblBookEditions.Add(bookEdition);

            // Single atomic save
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Create Book Edition Async => Book Edition is created successfully");
            return Result<BookEditionModel>.Success(new BookEditionModel
            {
                BookEditionId = bookEdition.BookEditionId,
                BookId = bookEdition.BookId,
                EditionId = bookEdition.EditionId,
                EditionName = editionType.EditionName,
                Isbn = bookEdition.Isbn,
                Price = bookEdition.Price,
                StockQuantity = bookEdition.StockQuantity,
                EditionNote = bookEdition.EditionNote,
                PublishDate = bookEdition.PublishDate,
                PageCount = bookEdition.PageCount,
                ReorderLevel = bookEdition.ReorderLevel,
                CoverImageUrl = bookEdition.CoverImageUrl,
                Publisher = bookEdition.Publisher,
                IsDeleted = bookEdition.IsDeleted
            }, "Created new book edition successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning("Create Book Edition Async => Failed to create book edition {Message}", ex.Message);
            return Result<BookEditionModel>.SystemError("Failed to create book edition: " + ex.Message);
        }
    }

    public async Task<Result<BookEditionModel>> UpdateBookEditionAsync(BookEditionPatchRequestModel requestModel)
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
                return Result<BookEditionModel>.NotFound("Book edition not found.");
            }

            if (requestModel.Price <= 0)
            {
                return Result<BookEditionModel>.ValidationError("Price must be greater than 0.");
            }
            if (requestModel.StockQuantity < 0)
            {
                return Result<BookEditionModel>.ValidationError("StockQuantity cannot be negative.");
            }

            // Handle ISBN uniqueness if changed
            if (!string.IsNullOrWhiteSpace(requestModel.Isbn) && requestModel.Isbn != bookEdition.Isbn)
            {
                var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                if (isbn.Length != 10 && isbn.Length != 13)
                    return Result<BookEditionModel>.ValidationError("ISBN must be 10 or 13 digits.");

                bool isbnExists = await _db.TblBookEditions.AnyAsync(e => e.Isbn == requestModel.Isbn && e.BookEditionId != requestModel.BookEditionId && !e.IsDeleted);
                if (isbnExists)
                    return Result<BookEditionModel>.Conflict("A book edition with this ISBN already exists.");
            }

            // Handle Edition Name change
            if (!string.IsNullOrWhiteSpace(requestModel.EditionName) && bookEdition.Edition?.EditionName != requestModel.EditionName)
            {
                var existingEditionNameCheck = await _db.TblBookEditions
                    .Include(be => be.Edition)
                    .FirstOrDefaultAsync(be => be.BookId == bookEdition.BookId && be.BookEditionId != bookEdition.BookEditionId && be.Edition != null && be.Edition.EditionName.ToLower() == requestModel.EditionName.ToLower() && !be.IsDeleted);

                if (existingEditionNameCheck != null)
                {
                    return Result<BookEditionModel>.Conflict("This book already has an edition with this name.");
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

            if (requestModel.PublishDate.HasValue && requestModel.PublishDate.Value.Date > DateTime.Today)
            {
                _logger.LogWarning("Patch Edition Async => Publication Date cannot be in the future");
                return Result<BookEditionModel>.ValidationError("Publication Date cannot be in the future.");
            }

            bookEdition.Price = requestModel.Price;
            bookEdition.StockQuantity = requestModel.StockQuantity;
            bookEdition.EditionNote = requestModel.EditionNote;
            bookEdition.Publisher = requestModel.Publisher;
            if (requestModel.PublishDate.HasValue)
            {
                bookEdition.PublishDate = requestModel.PublishDate.Value;
            }
            bookEdition.PageCount = requestModel.PageCount;
            bookEdition.ReorderLevel = requestModel.ReorderLevel;
            bookEdition.Isbn = string.IsNullOrWhiteSpace(requestModel.Isbn) ? null : requestModel.Isbn;
            if (requestModel.CoverImageUrl != null)
            {
                bookEdition.CoverImageUrl = requestModel.CoverImageUrl;
            }
            if (!_db.ChangeTracker.HasChanges())
            {
                await transaction.CommitAsync();
                return Result<BookEditionModel>.Success(new BookEditionModel
                {
                    BookEditionId = bookEdition.BookEditionId,
                    BookId = bookEdition.BookId,
                    EditionId = bookEdition.EditionId,
                    EditionName = bookEdition.Edition?.EditionName ?? "",
                    Isbn = bookEdition.Isbn,
                    Price = bookEdition.Price,
                    StockQuantity = bookEdition.StockQuantity,
                    EditionNote = bookEdition.EditionNote,
                    PublishDate = bookEdition.PublishDate,
                    PageCount = bookEdition.PageCount,
                    ReorderLevel = bookEdition.ReorderLevel,
                    CoverImageUrl = bookEdition.CoverImageUrl,
                    Publisher = bookEdition.Publisher,
                    IsDeleted = bookEdition.IsDeleted
                }, "No changes detected.");
            }

            bookEdition.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Update Book Edition Async => Book Edition updated successfully");
            return Result<BookEditionModel>.Success(new BookEditionModel
            {
                BookEditionId = bookEdition.BookEditionId,
                BookId = bookEdition.BookId,
                EditionId = bookEdition.EditionId,
                EditionName = requestModel.EditionName,
                Isbn = bookEdition.Isbn,
                Price = bookEdition.Price,
                StockQuantity = bookEdition.StockQuantity,
                EditionNote = bookEdition.EditionNote,
                PublishDate = bookEdition.PublishDate,
                PageCount = bookEdition.PageCount,
                ReorderLevel = bookEdition.ReorderLevel,
                CoverImageUrl = bookEdition.CoverImageUrl,
                Publisher = bookEdition.Publisher,
                IsDeleted = bookEdition.IsDeleted
            }, "Updated book edition successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning("Update Book Edition Async => Failed to update book edition {Message}", ex.Message);
            return Result<BookEditionModel>.SystemError("Failed to update book edition: " + ex.Message);
        }
    }

    public async Task<Result<BookEditionModel>> DeleteBookEditionAsync(BookEditionDeleteRequestModel requestModel)
    {
        _logger.LogInformation("Delete Book Edition Async => Deleting book edition");
        try
        {
            var bookEdition = await _db.TblBookEditions.FirstOrDefaultAsync(x => x.BookEditionId == requestModel.BookEditionId);
            if (bookEdition == null)
            {
                return Result<BookEditionModel>.NotFound("Book edition not found.");
            }

            if (bookEdition.IsDeleted)
            {
                return Result<BookEditionModel>.Conflict("Book edition is already deleted.");
            }

            bookEdition.IsDeleted = true;
            bookEdition.UpdatedAt = DateTime.Now;
            _db.Entry(bookEdition).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Delete Book Edition Async => Book Edition deleted successfully");
            return Result<BookEditionModel>.DeleteSuccess("Book edition deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Delete Book Edition Async => Failed to delete book edition {Message}", ex.Message);
            return Result<BookEditionModel>.SystemError("Failed to delete book edition: " + ex.Message);
        }
    }

    public async Task<Result<List<BookModel>>> GetTrendingBooksAsync(int days = 7, int limit = 6)
    {
        try
        {
            if (days <= 0) days = 7;
            if (limit <= 0) limit = 6;

            var startDate = DateTime.Now.AddDays(-days);

            // 1. Group completed order items in the last N days by BookId
            var topSoldBooks = await _db.TblOrderItems
                .AsNoTracking()
                .Where(oi => oi.BookEdition != null
                          && oi.BookEdition.Book != null
                          && !oi.BookEdition.Book.IsDeleted
                          && oi.Order != null
                          && oi.Order.OrderStatus == (byte)OrderStatus.Completed
                          && oi.Order.OrderDate >= startDate)
                .GroupBy(oi => oi.BookEdition!.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(limit)
                .ToListAsync();

            var topBookIds = topSoldBooks.Select(x => x.BookId).ToList();

            // 2. Fetch full book details for these top sellers
            var booksDict = await _db.TblBooks
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.TblBookEditions.Where(e => !e.IsDeleted))
                    .ThenInclude(e => e.Edition)
                .Where(b => topBookIds.Contains(b.BookId) && !b.IsDeleted)
                .ToDictionaryAsync(b => b.BookId);

            var resultList = new List<BookModel>();
            foreach (var id in topBookIds)
            {
                if (booksDict.TryGetValue(id, out var tblBook))
                {
                    resultList.Add(MapToBookModel(tblBook));
                }
            }

            // 3. Fallback / Backfill if sales are fewer than limit
            if (resultList.Count < limit)
            {
                var alreadyIds = resultList.Select(b => b.BookId).ToList();
                var fallbackBooks = await _db.TblBooks
                    .AsNoTracking()
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.TblBookEditions.Where(e => !e.IsDeleted))
                        .ThenInclude(e => e.Edition)
                    .Where(b => !b.IsDeleted 
                             && !alreadyIds.Contains(b.BookId)
                             && b.TblBookEditions.Any(e => !e.IsDeleted))
                    .OrderByDescending(b => b.BookId)
                    .Take(limit - resultList.Count)
                    .ToListAsync();

                foreach (var fb in fallbackBooks)
                {
                    resultList.Add(MapToBookModel(fb));
                }
            }

            return Result<List<BookModel>>.Success(resultList, "Trending books fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrendingBooksAsync => Exception occurred");
            return Result<List<BookModel>>.SystemError("Failed to fetch trending books: " + ex.Message);
        }
    }

    public async Task<Result<List<BookModel>>> GetBestsellerBooksAsync(int days = 30, int limit = 8)
    {
        try
        {
            if (limit <= 0) limit = 8;

            IQueryable<TblOrderItem> query = _db.TblOrderItems
                .AsNoTracking()
                .Where(oi => oi.BookEdition != null
                          && oi.BookEdition.Book != null
                          && !oi.BookEdition.Book.IsDeleted
                          && oi.Order != null
                          && oi.Order.OrderStatus == (byte)OrderStatus.Completed);

            if (days > 0)
            {
                var startDate = DateTime.Now.AddDays(-days);
                query = query.Where(oi => oi.Order.OrderDate >= startDate);
            }

            var topSoldBooks = await query
                .GroupBy(oi => oi.BookEdition!.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(limit)
                .ToListAsync();

            var topBookIds = topSoldBooks.Select(x => x.BookId).ToList();

            var booksDict = await _db.TblBooks
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.TblBookEditions.Where(e => !e.IsDeleted))
                    .ThenInclude(e => e.Edition)
                .Where(b => topBookIds.Contains(b.BookId) && !b.IsDeleted)
                .ToDictionaryAsync(b => b.BookId);

            var resultList = new List<BookModel>();
            foreach (var id in topBookIds)
            {
                if (booksDict.TryGetValue(id, out var tblBook))
                {
                    resultList.Add(MapToBookModel(tblBook));
                }
            }

            // Fallback / Backfill if sales are fewer than limit
            if (resultList.Count < limit)
            {
                var alreadyIds = resultList.Select(b => b.BookId).ToList();
                var fallbackBooks = await _db.TblBooks
                    .AsNoTracking()
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.TblBookEditions.Where(e => !e.IsDeleted))
                        .ThenInclude(e => e.Edition)
                    .Where(b => !b.IsDeleted 
                             && !alreadyIds.Contains(b.BookId)
                             && b.TblBookEditions.Any(e => !e.IsDeleted))
                    .OrderByDescending(b => b.BookId)
                    .Take(limit - resultList.Count)
                    .ToListAsync();

                foreach (var fb in fallbackBooks)
                {
                    resultList.Add(MapToBookModel(fb));
                }
            }

            return Result<List<BookModel>>.Success(resultList, "Bestseller books fetched successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBestsellerBooksAsync => Exception occurred");
            return Result<List<BookModel>>.SystemError("Failed to fetch bestseller books: " + ex.Message);
        }
    }

    private static BookModel MapToBookModel(TblBook item)
    {
        return new BookModel
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
            Editions = item.TblBookEditions.Where(e => !e.IsDeleted).Select(e => new BookEditionModel
            {
                BookEditionId = e.BookEditionId,
                BookId = e.BookId,
                EditionId = e.EditionId,
                EditionName = e.Edition?.EditionName ?? "Unknown Edition",
                Isbn = e.Isbn,
                Price = e.Price,
                StockQuantity = e.StockQuantity,
                EditionNote = e.EditionNote,
                PublishDate = e.PublishDate,
                PageCount = e.PageCount,
                ReorderLevel = e.ReorderLevel,
                CoverImageUrl = e.CoverImageUrl,
                Publisher = e.Publisher,
                IsDeleted = e.IsDeleted
            }).ToList()
        };
    }
}
