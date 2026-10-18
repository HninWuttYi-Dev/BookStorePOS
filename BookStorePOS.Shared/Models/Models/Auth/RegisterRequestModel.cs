using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Auth;

public class RegisterRequestModel
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponseModel : Result<string?>
{
    public string? AccessToken { get; set; }
    public string? Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public List<string> Permissions { get; set; } = new();
}

