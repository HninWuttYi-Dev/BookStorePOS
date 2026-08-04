namespace BookStorePOS.Domain.Features.UserFeatures.Models;

public class UserCreateRequestModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Customer";
}

public class UserCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public int? UserId { get; set; }
}
