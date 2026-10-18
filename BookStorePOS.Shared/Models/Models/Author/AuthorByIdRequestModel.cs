using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Author;

public class AuthorByIdRequestModel
{
    public int AuthorId { get; set; }
}

public class AuthorByIdResponseModel : Result<AuthorModel>
{
}
