using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Author;

public class AuthorDeleteRequestModel
{
    public int AuthorId { get; set; }
}

public class AuthorDeleteResponseModel : Result<AuthorModel>
{
}
