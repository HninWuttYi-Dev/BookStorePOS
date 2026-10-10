using System.Threading.Tasks;
using BookStorePOS.Shared.Models.User;

namespace BookStorePOS.Domain.Features.User;

public interface IUserService
{
    Task<UserCreateResponseModel> CreateUserAsync(UserCreateRequestModel request);
    Task<UserListResponseModel> GetUsersAsync(UserListRequestModel request);
    Task<UserGetByIdResponseModel> GetUserByIdAsync(int userId);
    Task<UserUpdateResponseModel> UpdateUserAsync(UserUpdateRequestModel request);
    Task<UserDeleteResponseModel> DeleteUserAsync(int userId);
}
