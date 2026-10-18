using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Edition;

public class EditionCreateRequestModel
{
    public string EditionName { get; set; } = null!;
}

public class EditionCreateResponseModel : Result<EditionModel>
{
}
