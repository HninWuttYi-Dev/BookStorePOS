using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Genre;
using BookStorePOS.Shared.Models.Genre;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenreController> _logger;

    public GenreController(IGenreService genreService, ILogger<GenreController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetGenresAsync([FromQuery] GenreListRequestModel requestModel)
    {
        _logger.LogInformation("Get Genres Async => Fetching all genres");
        var response = await _genreService.GetGenresAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Genres Async => Failed to fetch genres {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Genres Async => Genres fetched successfully");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreByIdAsync(int id)
    {
        _logger.LogInformation("Get Genre ById Async => Fetching genre by Id");
        var response = await _genreService.GetGenreByIdAsync(new GenreByIdRequestModel { GenreId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Genre ById Async => Failed to fetch genre {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Genre ById Async => Genre fetched successfully");
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenreAsync([FromBody] GenreCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Genre Async => Creating genre");
        var response = await _genreService.CreateGenreAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Create Genre Async => Failed to create genre {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Create Genre Async => Genre is created successfully");
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateGenreAsync(int id, [FromBody] GenrePatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Genre Async => Updating genre");
        requestModel.GenreId = id;
        var response = await _genreService.UpdateGenreAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Update Genre Async => Failed to update genre {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Update Genre Async => Genre is updated successfully");
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenreAsync(int id)
    {
        _logger.LogInformation("Delete Genre Async => Deleting genre");
        var response = await _genreService.DeleteGenreAsync(new GenreDeleteRequestModel { GenreId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Delete Genre Async => Failed to delete genre {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Delete Genre Async => Genre is deleted successfully");
        return Ok(response);
    }
}
