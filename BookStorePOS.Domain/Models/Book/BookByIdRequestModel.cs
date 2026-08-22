namespace BookStorePOS.Domain.Models.Book;

public class BookByIdRequestModel
{
    public int BookId { get; set; }
}

public class BookByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
