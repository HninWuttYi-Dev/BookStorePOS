using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleDeleteRequestModel
{
    public int RoleId { get; set; }
}

public class RoleDeleteResponseModel : Result<object?>
{
}
