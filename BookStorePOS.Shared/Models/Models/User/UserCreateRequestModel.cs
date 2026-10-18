using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.User;

public class UserCreateRequestModel
{
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PasswordHash { get; set; }
    public int? RoleId { get; set; }
}

public class UserCreateResponseModel : Result<UserModel>
{
}
