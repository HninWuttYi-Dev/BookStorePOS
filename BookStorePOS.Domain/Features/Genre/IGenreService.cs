using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Genre;

namespace BookStorePOS.Domain.Features.Genre;

public interface IGenreService
{
    Task<GenreListResponseModel> GetGenresAsync(GenreListRequestModel requestModel);
    Task<Result<GenreModel>> GetGenreByIdAsync(GenreByIdRequestModel requestModel);
    Task<Result<GenreModel>> CreateGenreAsync(GenreCreateRequestModel requestModel);
    Task<Result<GenreModel>> UpdateGenreAsync(GenrePatchRequestModel requestModel);
    Task<Result<GenreModel>> DeleteGenreAsync(GenreDeleteRequestModel requestModel);
}

