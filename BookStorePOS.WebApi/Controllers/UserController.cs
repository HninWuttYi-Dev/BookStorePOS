using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.User;
using BookStorePOS.Domain.Models.User;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController()
    {
        _userService = new UserService();
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var response = _userService.GetUsers(new UserListRequestModel());
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var response = _userService.GetUser(new UserEditRequestModel{UserId = id});
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequestModel requestModel)
    {
        var response = _userService.CreateUser(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserPatchRequestModel requestModel)
    {
        requestModel.UserId = id;
        var response = _userService.UpdateUser(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{UserId}")]
    public IActionResult DeleteUser([FromRoute] UserDeleteRequestModel requestModel)
    {
        var response = _userService.DeleteUser(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }
}
