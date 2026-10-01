using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.Author;

public class AuthorListRequestModel
{
    public string? AuthorName { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class AuthorListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<AuthorModel> Data { get; set; } = null!;
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Count { get; set; }
    public int TotalPages { get; set; }
}

public class AuthorModel
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
