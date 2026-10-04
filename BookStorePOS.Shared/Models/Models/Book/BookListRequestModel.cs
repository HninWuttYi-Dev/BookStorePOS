using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.Book;

public class BookListRequestModel
{
    public string? Isbn {get; set;}
    public string? Title {get; set;}
    public string? Author { get; set; }
    public int? AuthorId { get; set; }
    public string? Genre {get; set;}
    public int? GenreId { get; set; }
    public bool? OnlyLowStock {get; set;}
    public int Page {get; set;} = 1;
    public int Limit {get; set;} = 10;
    public string? SearchQuery { get; set; }
    
}

public class BookListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<BookModel> Data { get; set; } = null!;
    public int Page {get; set;}
    public int Limit {get; set;}
    public int Count {get;set;}
    public int TotalPages {get; set;}
}

public class BookModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    public int? AuthorId { get; set; }
    public string? Genre { get; set; }
    public int? GenreId { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    
    public decimal StartingPrice { get; set; }
    public int TotalStockQuantity { get; set; }

    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string? CoverImageUrl { get; set; }

    public List<BookEditionModel> Editions { get; set; } = new List<BookEditionModel>();
}

public class BookEditionModel
{
    public int BookEditionId { get; set; }
    public int BookId { get; set; }
    public int EditionId { get; set; }
    public string EditionName { get; set; } = null!;
    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsDeleted { get; set; }
}
