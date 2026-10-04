namespace BookStorePOS.Shared.Models.Edition;

public class EditionCreateRequestModel
{
    public string EditionName { get; set; } = null!;
}

public class EditionCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public EditionModel Data { get; set; } = null!;
}
