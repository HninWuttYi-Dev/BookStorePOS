using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleCreateRequestModel
{
    public string RoleName { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = new();
}

public class RoleCreateResponseModel : Result<RoleModel>
{
}
