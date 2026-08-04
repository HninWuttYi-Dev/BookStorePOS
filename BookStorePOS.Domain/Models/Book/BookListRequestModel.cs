using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.Book;

public class BookListRequestModel
{
}

public class BookListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<BookModel> Data { get; set; } = null!;
}

public class BookModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsDeleted { get; set; }
}
