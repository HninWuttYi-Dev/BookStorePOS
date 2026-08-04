using System.Collections.Generic;

namespace BookStorePOS.Domain.Features.UserFeatures.Models;

public class UserListRequestModel
{
}

public class UserListResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public List<UserModel>? Users { get; set; }
}

public class UserModel
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
