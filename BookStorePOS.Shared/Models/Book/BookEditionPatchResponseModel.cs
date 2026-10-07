namespace BookStorePOS.Shared.Models.Book;

public class BookEditionPatchResponseModel
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public BookEditionModel? Data { get; set; }
}
