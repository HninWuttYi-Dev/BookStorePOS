using System;
using System.Collections.Generic;

namespace BookStorePOS.ConsoleApp.Models;

public class BookModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BaseResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
}

public class BookListResponseModel : BaseResponseModel
{
    public List<BookModel> Data { get; set; } = new();
}

public class BookResponseModel : BaseResponseModel
{
    public BookModel Data { get; set; } = null!;
}

public class BookCreateRequestModel
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class BookPatchRequestModel
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
}
