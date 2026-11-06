using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Author;

public class AuthorListRequestModel
{
    public string? AuthorName { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class AuthorListResponseModel : Result<List<AuthorModel>>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Count { get; set; }
    public int TotalPages { get; set; }
}

public class AuthorCursorRequestModel
{
    public int? Cursor { get; set; }
    public int Limit { get; set; } = 6;
    public string? AuthorName { get; set; }
}

public class AuthorCursorResponseModel : Result<List<AuthorModel>>
{
    public int? NextCursor { get; set; }
    public bool HasNextPage { get; set; }
    public int TotalCount { get; set; }
}

public class AuthorModel
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int BookCount { get; set; }
    public List<AuthorBookPreviewModel> SampleBooks { get; set; } = new();
}

public class AuthorBookPreviewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal Price { get; set; }
}
