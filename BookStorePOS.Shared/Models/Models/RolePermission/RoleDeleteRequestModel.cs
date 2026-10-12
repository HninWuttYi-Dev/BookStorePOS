namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleDeleteRequestModel
{
    public int RoleId { get; set; }
}

public class RoleDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
