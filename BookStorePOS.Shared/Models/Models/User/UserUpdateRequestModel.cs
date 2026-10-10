namespace BookStorePOS.Shared.Models.User;

public class UserUpdateRequestModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
}

public class UserUpdateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public UserModel? Data { get; set; }
}
