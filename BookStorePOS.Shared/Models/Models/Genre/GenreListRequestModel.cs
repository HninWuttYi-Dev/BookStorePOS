using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Genre;

public class GenreListRequestModel
{
    public string? GenreName { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class GenreListResponseModel : Result<List<GenreModel>>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Count { get; set; }
    public int TotalPages { get; set; }
}

public class GenreModel
{
    public int GenreId { get; set; }
    public string GenreName { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
