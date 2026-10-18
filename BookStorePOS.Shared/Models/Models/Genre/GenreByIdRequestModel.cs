using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Genre;

public class GenreByIdRequestModel
{
    public int GenreId { get; set; }
}

public class GenreByIdResponseModel : Result<GenreModel>
{
}
