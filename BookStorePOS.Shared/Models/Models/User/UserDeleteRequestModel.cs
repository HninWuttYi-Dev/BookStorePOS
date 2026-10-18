using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.User;

public class UserDeleteRequestModel
{
    public int UserId { get; set; }
}

public class UserDeleteResponseModel : Result<UserModel>
{
}
