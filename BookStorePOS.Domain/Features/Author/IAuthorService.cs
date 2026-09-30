using System.Threading.Tasks;
using BookStorePOS.Domain.Models.Author;

namespace BookStorePOS.Domain.Features.Author;

public interface IAuthorService
{
    Task<AuthorListResponseModel> GetAuthorsAsync(AuthorListRequestModel requestModel);
    Task<AuthorByIdResponseModel> GetAuthorByIdAsync(AuthorByIdRequestModel requestModel);
    Task<AuthorCreateResponseModel> CreateAuthorAsync(AuthorCreateRequestModel requestModel);
    Task<AuthorPatchResponseModel> UpdateAuthorAsync(AuthorPatchRequestModel requestModel);
    Task<AuthorDeleteResponseModel> DeleteAuthorAsync(AuthorDeleteRequestModel requestModel);
}
