namespace BookStorePOS.Shared.Models.User;

public class ResetStaffPasswordRequestModel
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
