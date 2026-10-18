using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.User;

public class UserGetByIdRequestModel
{
    public int UserId { get; set; }
}

public class UserGetByIdResponseModel : Result<UserModel>
{
}
