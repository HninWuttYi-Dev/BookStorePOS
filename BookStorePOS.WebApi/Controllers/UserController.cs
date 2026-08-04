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
       return Ok(_userService.GetUsers(new UserListRequestModel()));
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        return Ok(_userService.GetUser(new UserEditRequestModel{UserId = id}));
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequestModel requestModel)
    {
       return Ok(_userService.CreateUser(requestModel));
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UserPatchRequestModel requestModel)
    {
        requestModel.UserId = id;
        return Ok(_userService.UpdateUser(requestModel));
    }

    [HttpDelete("{UserId}")]
    public IActionResult DeleteUser([FromRoute] UserDeleteRequestModel requestModel)
    {
        return Ok(_userService.DeleteUser(requestModel));
    }
}
