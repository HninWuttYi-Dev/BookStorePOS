namespace BookStorePOS.Shared.Models.Genre;

public class GenreByIdRequestModel
{
    public int GenreId { get; set; }
}

public class GenreByIdResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public GenreModel Data { get; set; } = null!;
}
