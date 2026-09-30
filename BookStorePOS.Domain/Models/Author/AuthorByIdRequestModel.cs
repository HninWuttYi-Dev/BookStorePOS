namespace BookStorePOS.Domain.Models.Author;

public class AuthorByIdRequestModel
{
    public int AuthorId { get; set; }
}

public class AuthorByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public AuthorModel Data { get; set; } = null!;
}
