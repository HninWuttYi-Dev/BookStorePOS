namespace BookStorePOS.Shared.Models.Edition;

public class EditionByIdRequestModel
{
    public int EditionId { get; set; }
}

public class EditionByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public EditionModel Data { get; set; } = null!;
}
