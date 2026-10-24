using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Auth;

public class ForgotPasswordRequestModel
{
    public string Phone { get; set; } = string.Empty;
}

public class ForgotPasswordResponseData
{
    public string? OtpCode { get; set; }
    public int ExpiresInMinutes { get; set; } = 5;
}

public class ForgotPasswordResponseModel : Result<ForgotPasswordResponseData?>
{
}
