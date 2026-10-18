using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Genre;

public class GenreDeleteRequestModel
{
    public int GenreId { get; set; }
}

public class GenreDeleteResponseModel : Result<GenreModel>
{
}
