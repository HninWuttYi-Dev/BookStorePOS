using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.User;
using BookStorePOS.Shared.Models.User;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] UserListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Users Async => Fetching all users");
            var response = await _userService.GetUsersAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Get Users Async => Failed to fetch users {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Get Users Async => Users fetched successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUsersAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get User ById Async => Fetching user by Id");
            var response = await _userService.GetUserByIdAsync(id);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Get User ById Async => Failed to fetch user {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Get User ById Async => User fetched successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserByIdAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create User Async => Creating user");
            var response = await _userService.CreateUserAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Create User Async => Failed to create user {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Create User Async => User is created successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateUserAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserUpdateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update User Async => Updating user");
            requestModel.UserId = id;
            var response = await _userService.UpdateUserAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Update User Async => Failed to update user {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Update User Async => User is updated successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateUserAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete User Async => Deleting user");
            var response = await _userService.DeleteUserAsync(id);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Delete User Async => Failed to delete user {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Delete User Async => User is deleted successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteUserAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
}
