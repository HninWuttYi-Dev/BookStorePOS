using System.Threading.Tasks;
using BookStorePOS.Domain.Models.Genre;

namespace BookStorePOS.Domain.Features.Genre;

public interface IGenreService
{
    Task<GenreListResponseModel> GetGenresAsync(GenreListRequestModel requestModel);
    Task<GenreByIdResponseModel> GetGenreByIdAsync(GenreByIdRequestModel requestModel);
    Task<GenreCreateResponseModel> CreateGenreAsync(GenreCreateRequestModel requestModel);
    Task<GenrePatchResponseModel> UpdateGenreAsync(GenrePatchRequestModel requestModel);
    Task<GenreDeleteResponseModel> DeleteGenreAsync(GenreDeleteRequestModel requestModel);
}
