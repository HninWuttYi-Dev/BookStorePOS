namespace BookStorePOS.Domain.Features.UserFeatures.Models;

public class UserDeleteRequestModel
{
    public int UserId { get; set; }
}

public class UserDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
}
