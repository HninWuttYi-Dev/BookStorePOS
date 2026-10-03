namespace BookStorePOS.Shared.Models.Genre;

public class GenreCreateRequestModel
{
    public string GenreName { get; set; } = null!;
}

public class GenreCreateResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public GenreModel Data { get; set; } = null!;
}
