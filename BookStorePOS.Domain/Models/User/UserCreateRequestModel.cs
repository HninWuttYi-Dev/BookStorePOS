namespace BookStorePOS.Domain.Models.User;

public class UserCreateRequestModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Customer";
}

public class UserCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public UserModel Data { get; set; } = null!;
}
