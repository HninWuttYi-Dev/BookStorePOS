using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Author;

public class AuthorCreateRequestModel
{
    public string AuthorName { get; set; } = null!;
}

public class AuthorCreateResponseModel : Result<AuthorModel>
{
}
