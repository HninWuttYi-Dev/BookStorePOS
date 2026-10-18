using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleByIdRequestModel
{
    public int RoleId { get; set; }
}

public class RoleByIdResponseModel : Result<RoleModel>
{
}
