using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Edition;

public class EditionDeleteRequestModel
{
    public int EditionId { get; set; }
}

public class EditionDeleteResponseModel : Result<EditionModel>
{
}
