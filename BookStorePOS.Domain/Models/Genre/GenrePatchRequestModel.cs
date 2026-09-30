namespace BookStorePOS.Domain.Models.Genre;

public class GenrePatchRequestModel
{
    public int GenreId { get; set; }
    public string? GenreName { get; set; }
}

public class GenrePatchResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public GenreModel Data { get; set; } = null!;
}
