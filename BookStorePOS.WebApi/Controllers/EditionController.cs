using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Edition;
using BookStorePOS.Shared.Models.Edition;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditionController : ControllerBase
{
    private readonly IEditionService _editionService;
    private readonly ILogger<EditionController> _logger;

    public EditionController(IEditionService editionService, ILogger<EditionController> logger)
    {
        _editionService = editionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEditionsAsync([FromQuery] EditionListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Editions Async => Fetching all editions");
            var response = await _editionService.GetEditionsAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Get Editions Async => Failed to fetch editions {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Get Editions Async => Editions fetched successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetEditionsAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEditionByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get Edition ById Async => Fetching edition by Id");
            var response = await _editionService.GetEditionByIdAsync(new EditionByIdRequestModel { EditionId = id });
            if (!response.isSuccess)
            {
                _logger.LogWarning("Get Edition ById Async => Failed to fetch edition {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Get Edition ById Async => Edition fetched successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetEditionByIdAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateEditionAsync([FromBody] EditionCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create Edition Async => Creating edition");
            var response = await _editionService.CreateEditionAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Create Edition Async => Failed to create edition {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Create Edition Async => Edition is created successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateEditionAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateEditionAsync(int id, [FromBody] EditionPatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update Edition Async => Updating edition");
            requestModel.EditionId = id;
            var response = await _editionService.UpdateEditionAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Update Edition Async => Failed to update edition {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Update Edition Async => Edition is updated successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateEditionAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEditionAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete Edition Async => Deleting edition");
            var response = await _editionService.DeleteEditionAsync(new EditionDeleteRequestModel { EditionId = id });
            if (!response.isSuccess)
            {
                _logger.LogWarning("Delete Edition Async => Failed to delete edition {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Delete Edition Async => Edition is deleted successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteEditionAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
}
