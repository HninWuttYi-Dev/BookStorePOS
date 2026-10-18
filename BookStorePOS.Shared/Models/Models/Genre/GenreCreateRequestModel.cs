using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Genre;

public class GenreCreateRequestModel
{
    public string GenreName { get; set; } = null!;
}

public class GenreCreateResponseModel : Result<GenreModel>
{
}
