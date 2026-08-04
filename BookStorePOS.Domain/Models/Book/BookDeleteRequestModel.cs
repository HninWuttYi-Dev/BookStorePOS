namespace BookStorePOS.Domain.Models.Book;

public class BookDeleteRequestModel
{
    public int BookId { get; set; }
}

public class BookDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
