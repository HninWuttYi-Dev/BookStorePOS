namespace BookStorePOS.Shared.Models.Book;

public class BookPatchRequestModel
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int? AuthorId { get; set; }
    public string? Genre { get; set; }
    public int? GenreId { get; set; }
    public string? Description { get; set; }
    
    public string? Isbn { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public int? ReorderLevel { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class BookPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
