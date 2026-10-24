using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Auth;

public class ResetPasswordRequestModel
{
    public string Phone { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ResetPasswordResponseModel : Result<string?>
{
}
