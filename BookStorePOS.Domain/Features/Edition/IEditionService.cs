using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Edition;

namespace BookStorePOS.Domain.Features.Edition;

public interface IEditionService
{
    Task<EditionListResponseModel> GetEditionsAsync(EditionListRequestModel requestModel);
    Task<EditionByIdResponseModel> GetEditionByIdAsync(EditionByIdRequestModel requestModel);
    Task<EditionCreateResponseModel> CreateEditionAsync(EditionCreateRequestModel requestModel);
    Task<EditionPatchResponseModel> UpdateEditionAsync(EditionPatchRequestModel requestModel);
    Task<EditionDeleteResponseModel> DeleteEditionAsync(EditionDeleteRequestModel requestModel);
}
