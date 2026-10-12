using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionModel> Permissions { get; set; } = new();
}

public class PermissionModel
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class RoleListRequestModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class RoleListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<RoleModel> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
