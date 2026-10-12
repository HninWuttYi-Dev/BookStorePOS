using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.RolePermission;

public class AssignRolePermissionsRequestModel
{
    public int RoleId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}

public class AssignRolePermissionsResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
