using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.User;

namespace BookStorePOS.Domain.Features.User;

public interface IUserService
{
    Task<Result<UserModel>> CreateUserAsync(UserCreateRequestModel request);
    Task<UserListResponseModel> GetUsersAsync(UserListRequestModel request);
    Task<Result<UserModel>> GetUserByIdAsync(int userId);
    Task<Result<UserModel>> UpdateUserAsync(UserUpdateRequestModel request);
    Task<Result<bool>> ResetPasswordAsync(int userId, string newPassword);
    Task<Result<UserModel>> DeleteUserAsync(int userId);
}

