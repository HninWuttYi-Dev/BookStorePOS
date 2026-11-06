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

public class GenreCursorRequestModel
{
    public int? Cursor { get; set; }
    public int Limit { get; set; } = 6;
    public string? GenreName { get; set; }
}

public class GenreCursorResponseModel : Result<List<GenreModel>>
{
    public int? NextCursor { get; set; }
    public bool HasNextPage { get; set; }
    public int TotalCount { get; set; }
}

public class GenreModel
{
    public int GenreId { get; set; }
    public string GenreName { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int BookCount { get; set; }
    public List<GenreBookPreviewModel> SampleBooks { get; set; } = new();
}

public class GenreBookPreviewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal Price { get; set; }
}
