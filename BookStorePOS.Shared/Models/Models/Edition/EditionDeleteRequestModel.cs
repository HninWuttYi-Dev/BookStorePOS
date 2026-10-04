namespace BookStorePOS.Shared.Models.Edition;

public class EditionDeleteRequestModel
{
    public int EditionId { get; set; }
}

public class EditionDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public EditionModel Data { get; set; } = null!;
}
