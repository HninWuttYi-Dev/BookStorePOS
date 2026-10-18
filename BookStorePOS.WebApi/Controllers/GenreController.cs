using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Genre;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Genre;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class GenreController : BaseController
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenreController> _logger;

    public GenreController(IGenreService genreService, ILogger<GenreController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetGenresAsync([FromQuery] GenreListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Genres Async => Fetching all genres");
            var response = await _genreService.GetGenresAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGenresAsync => Exception occurred");
            return ExecuteResult(Result<List<GenreModel>>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGenreByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get Genre ById Async => Fetching genre by Id {GenreId}", id);
            var response = await _genreService.GetGenreByIdAsync(new GenreByIdRequestModel { GenreId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGenreByIdAsync => Exception occurred");
            return ExecuteResult(Result<GenreModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPost]
    [Permission("Genre.Create")]
    public async Task<IActionResult> CreateGenreAsync([FromBody] GenreCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create Genre Async => Creating genre");
            var response = await _genreService.CreateGenreAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateGenreAsync => Exception occurred");
            return ExecuteResult(Result<GenreModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPatch("{id}")]
    [Permission("Genre.Update")]
    public async Task<IActionResult> UpdateGenreAsync(int id, [FromBody] GenrePatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update Genre Async => Updating genre {GenreId}", id);
            requestModel.GenreId = id;
            var response = await _genreService.UpdateGenreAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateGenreAsync => Exception occurred");
            return ExecuteResult(Result<GenreModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Permission("Genre.Delete")]
    public async Task<IActionResult> DeleteGenreAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete Genre Async => Deleting genre {GenreId}", id);
            var response = await _genreService.DeleteGenreAsync(new GenreDeleteRequestModel { GenreId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteGenreAsync => Exception occurred");
            return ExecuteResult(Result<GenreModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
