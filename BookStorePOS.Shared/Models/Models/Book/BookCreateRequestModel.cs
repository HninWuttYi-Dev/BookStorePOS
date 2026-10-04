namespace BookStorePOS.Shared.Models.Book;

public class BookCreateRequestModel
{
    public string? Isbn { get; set; }
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    public int? AuthorId { get; set; }
    public string? Genre { get; set; }
    public int? GenreId { get; set; }
    public string? Description { get; set; }
    
    // First Edition details
    public string EditionName { get; set; } = "1st Edition";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public string? CoverImageUrl { get; set; }
}

public class BookCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
