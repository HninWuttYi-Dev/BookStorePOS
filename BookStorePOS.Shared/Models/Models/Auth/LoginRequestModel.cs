using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Auth;

public class LoginRequestModel
{
    public string? Username { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseModel : Result<string?>
{
    public string AccessToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public List<string> Permissions { get; set; } = new();
}

