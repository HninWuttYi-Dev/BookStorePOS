using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class AssignRolePermissionsRequestModel
{
    public int RoleId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}

public class AssignRolePermissionsResponseModel : Result<object?>
{
}
