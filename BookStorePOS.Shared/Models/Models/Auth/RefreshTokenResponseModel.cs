using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Auth;

public class RefreshTokenResponseModel : Result<string?>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public List<string> Permissions { get; set; } = new();
}
