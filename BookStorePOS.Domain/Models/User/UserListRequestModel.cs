using System.Collections.Generic;

namespace BookStorePOS.Domain.Models.User;

public class UserListRequestModel
{
}

public class UserListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<UserModel> Data { get; set; } = null!;
}

public class UserModel
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
