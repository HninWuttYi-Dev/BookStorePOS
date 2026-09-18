using BookStorePOS.Domain.Models.Book;

namespace BookStorePOS.Domain.Features.Book;

public interface IBookService
{
    Task<BookCreateResponseModel> CreateBookAsync(BookCreateRequestModel requestModel);
    Task<BookDeleteResponseModel> DeleteBookAsync(BookDeleteRequestModel requestModel);
    Task<BookByIdResponseModel> GetBookAsync(BookByIdRequestModel requestModel);
    Task<BookListResponseModel> GetBooksAsync(BookListRequestModel requestModel);
    Task<BookPatchResponseModel> UpdateBookAsync(BookPatchRequestModel requestModel);
}
