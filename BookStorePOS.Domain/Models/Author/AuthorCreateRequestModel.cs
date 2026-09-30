namespace BookStorePOS.Domain.Models.Author;

public class AuthorCreateRequestModel
{
    public string AuthorName { get; set; } = null!;
}

public class AuthorCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public AuthorModel Data { get; set; } = null!;
}
