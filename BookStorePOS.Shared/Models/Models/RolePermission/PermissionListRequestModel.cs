using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.RolePermission;

public class PermissionListRequestModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PermissionListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<PermissionModel> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
