namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleByIdRequestModel
{
    public int RoleId { get; set; }
}

public class RoleByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public RoleModel? Data { get; set; }
}
