using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Edition;

public class EditionPatchRequestModel
{
    public int EditionId { get; set; }
    public string? EditionName { get; set; }
}

public class EditionPatchResponseModel : Result<EditionModel>
{
}
