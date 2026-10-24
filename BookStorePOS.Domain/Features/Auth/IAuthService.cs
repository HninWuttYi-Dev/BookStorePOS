using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Auth;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Domain.Features.Auth;

public interface IAuthService
{
    Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
    Task<RegisterResponseModel> RegisterCustomerAsync(RegisterRequestModel request);
    Task<RefreshTokenResponseModel> RefreshTokenAsync(RefreshTokenRequestModel request);
    Task<Result> LogoutAsync(string? refreshToken);
    Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ForgotPasswordRequestModel request);
    Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetPasswordRequestModel request);
}
