using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleUpdateRequestModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = new();
}

public class RoleUpdateResponseModel : Result<RoleModel>
{
}
