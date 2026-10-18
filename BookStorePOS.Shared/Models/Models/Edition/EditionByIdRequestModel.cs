using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Edition;

public class EditionByIdRequestModel
{
    public int EditionId { get; set; }
}

public class EditionByIdResponseModel : Result<EditionModel>
{
}
