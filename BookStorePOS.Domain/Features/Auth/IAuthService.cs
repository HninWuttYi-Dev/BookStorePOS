using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Auth;

namespace BookStorePOS.Domain.Features.Auth;

public interface IAuthService
{
    Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
    Task<RegisterResponseModel> RegisterCustomerAsync(RegisterRequestModel request);
}
