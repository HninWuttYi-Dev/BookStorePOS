using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Author;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Domain.Features.Author;

public interface IAuthorService
{
    Task<AuthorListResponseModel> GetAuthorsAsync(AuthorListRequestModel requestModel);
    Task<Result<AuthorModel>> GetAuthorByIdAsync(AuthorByIdRequestModel requestModel);
    Task<Result<AuthorModel>> CreateAuthorAsync(AuthorCreateRequestModel requestModel);
    Task<Result<AuthorModel>> UpdateAuthorAsync(AuthorPatchRequestModel requestModel);
    Task<Result<AuthorModel>> DeleteAuthorAsync(AuthorDeleteRequestModel requestModel);
}

