using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Book;

public class BookDeleteRequestModel
{
    public int BookId { get; set; }
}

public class BookDeleteResponseModel : Result<BookModel>
{
}
