using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Edition;

namespace BookStorePOS.Domain.Features.Edition;

public interface IEditionService
{
    Task<EditionListResponseModel> GetEditionsAsync(EditionListRequestModel requestModel);
    Task<Result<EditionModel>> GetEditionByIdAsync(EditionByIdRequestModel requestModel);
    Task<Result<EditionModel>> CreateEditionAsync(EditionCreateRequestModel requestModel);
    Task<Result<EditionModel>> UpdateEditionAsync(EditionPatchRequestModel requestModel);
    Task<Result<EditionModel>> DeleteEditionAsync(EditionDeleteRequestModel requestModel);
}

