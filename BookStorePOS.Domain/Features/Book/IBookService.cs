using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Book;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Domain.Features.Book;

public interface IBookService
{
    Task<Result<BookModel>> CreateBookAsync(BookCreateRequestModel requestModel);
    Task<Result<BookModel>> DeleteBookAsync(BookDeleteRequestModel requestModel);
    Task<Result<BookModel>> GetBookAsync(BookByIdRequestModel requestModel);
    Task<BookListResponseModel> GetBooksAsync(BookListRequestModel requestModel);
    Task<BookListResponseModel> GetLowStockBooksAsync();
    Task<Result<BookModel>> UpdateBookAsync(BookPatchRequestModel requestModel);
    Task<Result<BookEditionModel>> CreateBookEditionAsync(BookEditionCreateRequestModel requestModel);
    Task<Result<BookEditionModel>> UpdateBookEditionAsync(BookEditionPatchRequestModel requestModel);
    Task<Result<BookEditionModel>> DeleteBookEditionAsync(BookEditionDeleteRequestModel requestModel);
}
