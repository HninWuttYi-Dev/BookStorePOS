namespace BookStorePOS.Domain.Models.Book;

public class BookCreateRequestModel
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class BookCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
