using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Author;

public class AuthorPatchRequestModel
{
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
}

public class AuthorPatchResponseModel : Result<AuthorModel>
{
}
