namespace BookStorePOS.Domain.Features.UserFeatures.Models;

public class UserPatchRequestModel
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}

public class UserPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
}
