using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Book;

public class BookByIdRequestModel
{
    public int BookId { get; set; }
}

public class BookByIdResponseModel : Result<BookModel>
{
}
