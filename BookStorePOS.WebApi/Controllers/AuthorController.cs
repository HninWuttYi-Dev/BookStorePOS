using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Author;
using BookStorePOS.Domain.Models.Author;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorController> _logger;

    public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthorsAsync([FromQuery] AuthorListRequestModel requestModel)
    {
        _logger.LogInformation("Get Authors Async => Fetching all authors");
        var response = await _authorService.GetAuthorsAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Authors Async => Failed to fetch authors {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Authors Async => Authors fetched successfully");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorByIdAsync(int id)
    {
        _logger.LogInformation("Get Author ById Async => Fetching author by Id");
        var response = await _authorService.GetAuthorByIdAsync(new AuthorByIdRequestModel { AuthorId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Author ById Async => Failed to fetch author {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Author ById Async => Author fetched successfully");
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuthorAsync([FromBody] AuthorCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Author Async => Creating author");
        var response = await _authorService.CreateAuthorAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Create Author Async => Failed to create author {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Create Author Async => Author is created successfully");
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] AuthorPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Author Async => Updating author");
        requestModel.AuthorId = id;
        var response = await _authorService.UpdateAuthorAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Update Author Async => Failed to update author {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Update Author Async => Author is updated successfully");
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthorAsync(int id)
    {
        _logger.LogInformation("Delete Author Async => Deleting author");
        var response = await _authorService.DeleteAuthorAsync(new AuthorDeleteRequestModel { AuthorId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Delete Author Async => Failed to delete author {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Delete Author Async => Author is deleted successfully");
        return Ok(response);
    }
}
