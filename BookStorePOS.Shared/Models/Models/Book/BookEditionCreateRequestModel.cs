namespace BookStorePOS.Shared.Models.Book;

public class BookEditionCreateRequestModel
{
    public int BookId { get; set; }
    public string EditionName { get; set; } = null!;
    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public string? CoverImageUrl { get; set; }
}

public class BookEditionCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookEditionModel Data { get; set; } = null!;
}
