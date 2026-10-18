using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Author;
using BookStorePOS.Shared.Models.Author;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class AuthorController : BaseController
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuthorsAsync([FromQuery] AuthorListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Authors Async => Fetching all authors");
            var response = await _authorService.GetAuthorsAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthorsAsync => Exception occurred");
            return ExecuteResult(Result<List<AuthorModel>>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuthorByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get Author ById Async => Fetching author by Id {AuthorId}", id);
            var response = await _authorService.GetAuthorByIdAsync(new AuthorByIdRequestModel { AuthorId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthorByIdAsync => Exception occurred");
            return ExecuteResult(Result<AuthorModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPost]
    [Permission("Author.Create")]
    public async Task<IActionResult> CreateAuthorAsync([FromBody] AuthorCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create Author Async => Creating author");
            var response = await _authorService.CreateAuthorAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAuthorAsync => Exception occurred");
            return ExecuteResult(Result<AuthorModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPatch("{id}")]
    [Permission("Author.Update")]
    public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] AuthorPatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update Author Async => Updating author {AuthorId}", id);
            requestModel.AuthorId = id;
            var response = await _authorService.UpdateAuthorAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAuthorAsync => Exception occurred");
            return ExecuteResult(Result<AuthorModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Permission("Author.Delete")]
    public async Task<IActionResult> DeleteAuthorAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete Author Async => Deleting author {AuthorId}", id);
            var response = await _authorService.DeleteAuthorAsync(new AuthorDeleteRequestModel { AuthorId = id });
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAuthorAsync => Exception occurred");
            return ExecuteResult(Result<AuthorModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
