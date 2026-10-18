using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Edition;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Edition;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class EditionController : BaseController
{
    private readonly IEditionService _editionService;
    private readonly ILogger<EditionController> _logger;

    public EditionController(IEditionService editionService, ILogger<EditionController> logger)
    {
        _editionService = editionService;
        _logger = logger;
    }

    [HttpGet]
    [Permission("Edition.View")]
    public async Task<IActionResult> GetEditionsAsync([FromQuery] EditionListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Editions Async => Fetching all editions");
            var response = await _editionService.GetEditionsAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetEditionsAsync => Exception occurred");
            return ExecuteResult(Result<List<EditionModel>>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [Permission("Edition.View")]
    public async Task<IActionResult> GetEditionByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get Edition ById Async => Fetching edition by Id {EditionId}", id);
            var response = await _editionService.GetEditionByIdAsync(new EditionByIdRequestModel { EditionId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetEditionByIdAsync => Exception occurred");
            return ExecuteResult(Result<EditionModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPost]
    [Permission("Edition.Create")]
    public async Task<IActionResult> CreateEditionAsync([FromBody] EditionCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create Edition Async => Creating edition");
            var response = await _editionService.CreateEditionAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateEditionAsync => Exception occurred");
            return ExecuteResult(Result<EditionModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPatch("{id}")]
    [Permission("Edition.Update")]
    public async Task<IActionResult> UpdateEditionAsync(int id, [FromBody] EditionPatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update Edition Async => Updating edition {EditionId}", id);
            requestModel.EditionId = id;
            var response = await _editionService.UpdateEditionAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateEditionAsync => Exception occurred");
            return ExecuteResult(Result<EditionModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Permission("Edition.Delete")]
    public async Task<IActionResult> DeleteEditionAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete Edition Async => Deleting edition {EditionId}", id);
            var response = await _editionService.DeleteEditionAsync(new EditionDeleteRequestModel { EditionId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteEditionAsync => Exception occurred");
            return ExecuteResult(Result<EditionModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
