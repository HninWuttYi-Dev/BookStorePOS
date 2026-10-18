using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Edition;

public class EditionListRequestModel
{
    public string? EditionName { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class EditionListResponseModel : Result<List<EditionModel>>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Count { get; set; }
    public int TotalPages { get; set; }
}

public class EditionModel
{
    public int EditionId { get; set; }
    public string EditionName { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
