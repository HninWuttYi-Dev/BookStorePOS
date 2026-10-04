namespace BookStorePOS.Shared.Models.Edition;

public class EditionPatchRequestModel
{
    public int EditionId { get; set; }
    public string? EditionName { get; set; }
}

public class EditionPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public EditionModel Data { get; set; } = null!;
}
