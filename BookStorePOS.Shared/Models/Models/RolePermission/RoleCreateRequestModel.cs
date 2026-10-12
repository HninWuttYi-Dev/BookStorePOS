using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleCreateRequestModel
{
    public string RoleName { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = new();
}

public class RoleCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public RoleModel? Data { get; set; }
}
