using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Genre;

public class GenrePatchRequestModel
{
    public int GenreId { get; set; }
    public string? GenreName { get; set; }
}

public class GenrePatchResponseModel : Result<GenreModel>
{
}
