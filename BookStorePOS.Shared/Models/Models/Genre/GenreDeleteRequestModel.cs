namespace BookStorePOS.Shared.Models.Genre;

public class GenreDeleteRequestModel
{
    public int GenreId { get; set; }
}

public class GenreDeleteResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public GenreModel Data { get; set; } = null!;
}
