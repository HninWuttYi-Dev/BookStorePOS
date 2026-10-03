namespace BookStorePOS.Shared.Models.Author;

public class AuthorPatchRequestModel
{
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
}

public class AuthorPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public AuthorModel Data { get; set; } = null!;
}
