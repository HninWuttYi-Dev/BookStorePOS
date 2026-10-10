namespace BookStorePOS.Shared.Models.User;

public class UserGetByIdRequestModel
{
    public int UserId { get; set; }
}

public class UserGetByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public UserModel? Data { get; set; }
}
