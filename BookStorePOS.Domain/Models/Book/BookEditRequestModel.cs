namespace BookStorePOS.Domain.Models.Book;

public class BookEditRequestModel
{
    public int BookId { get; set; }
}

public class BookEditResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public BookModel Data { get; set; } = null!;
}
