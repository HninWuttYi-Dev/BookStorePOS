namespace BookStorePOS.Domain.Features.UserFeatures.Models;

public class UserEditRequestModel
{
    public int UserId { get; set; }
}

public class UserEditResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public int? UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}
