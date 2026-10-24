using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.User;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.User;
using BookStorePOS.WebApi.Filters;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Permission("User.View")]
    public async Task<IActionResult> GetUsersAsync([FromQuery] UserListRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Get Users Async => Fetching all users");
            var response = await _userService.GetUsersAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUsersAsync => Exception occurred");
            return ExecuteResult(Result<List<UserModel>>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [Permission("User.View")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Get User ById Async => Fetching user by Id {UserId}", id);
            var response = await _userService.GetUserByIdAsync(id);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserByIdAsync => Exception occurred");
            return ExecuteResult(Result<UserModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPost]
    [Permission("User.Create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Create User Async => Creating user");
            var response = await _userService.CreateUserAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateUserAsync => Exception occurred");
            return ExecuteResult(Result<UserModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPatch("{id}")]
    [Permission("User.Update")]
    public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserUpdateRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Update User Async => Updating user {UserId}", id);
            requestModel.UserId = id;
            var response = await _userService.UpdateUserAsync(requestModel);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateUserAsync => Exception occurred");
            return ExecuteResult(Result<UserModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpPost("{id}/reset-password")]
    [Permission("User.Update")]
    public async Task<IActionResult> ResetPasswordAsync(int id, [FromBody] ResetStaffPasswordRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Reset Password Async => Resetting password for user {UserId}", id);
            var response = await _userService.ResetPasswordAsync(id, requestModel.NewPassword);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResetPasswordAsync => Exception occurred");
            return ExecuteResult(Result<bool>.SystemError("Internal Server Error: " + ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Permission("User.Delete")]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        try
        {
            _logger.LogInformation("Delete User Async => Deleting user {UserId}", id);
            var response = await _userService.DeleteUserAsync(id);
            return ExecuteResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteUserAsync => Exception occurred");
            return ExecuteResult(Result<UserModel>.SystemError("Internal Server Error: " + ex.Message));
        }
    }
}
