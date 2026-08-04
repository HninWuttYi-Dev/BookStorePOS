namespace BookStorePOS.Domain.Models;

public class UserEditRequestModel
{
    public int UserId { get; set; }
}

public class UserEditResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public UserModel Data { get; set; } = null!;
}
