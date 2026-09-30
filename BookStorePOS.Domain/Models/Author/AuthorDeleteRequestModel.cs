namespace BookStorePOS.Domain.Models.Author;

public class AuthorDeleteRequestModel
{
    public int AuthorId { get; set; }
}

public class AuthorDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public AuthorModel Data { get; set; } = null!;
}
