namespace BookStorePOS.Domain.Models.Book;

public class BookPatchRequestModel
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
}

public class BookPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
