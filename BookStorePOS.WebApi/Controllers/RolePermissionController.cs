using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Domain.Features.RolePermission;
using BookStorePOS.Shared.Models.RolePermission;
using BookStorePOS.WebApi.Filters;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly ILogger<RolePermissionController> _logger;

    public RolePermissionController(
        IRolePermissionService rolePermissionService,
        ILogger<RolePermissionController> logger)
    {
        _rolePermissionService = rolePermissionService;
        _logger = logger;
    }

    [HttpGet("roles")]
    [Permission("Role.View")]
    public async Task<IActionResult> GetRoles([FromQuery] RoleListRequestModel request)
    {
        _logger.LogInformation("GetRoles => Fetching roles");
        var response = await _rolePermissionService.GetRolesAsync(request);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("roles/{id:int}")]
    [Permission("Role.View")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        _logger.LogInformation("GetRoleById => Fetching role id: {RoleId}", id);
        var response = await _rolePermissionService.GetRoleByIdAsync(id);
        if (!response.isSuccess)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpPost("roles")]
    [Permission("Role.Create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequestModel request)
    {
        _logger.LogInformation("CreateRole => Creating role: {RoleName}", request.RoleName);
        var response = await _rolePermissionService.CreateRoleAsync(request);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPut("roles/{id:int}")]
    [Permission("Role.Update")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateRequestModel request)
    {
        _logger.LogInformation("UpdateRole => Updating role id: {RoleId}", id);
        request.RoleId = id;
        var response = await _rolePermissionService.UpdateRoleAsync(request);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpDelete("roles/{id:int}")]
    [Permission("Role.Delete")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        _logger.LogInformation("DeleteRole => Deleting role id: {RoleId}", id);
        var response = await _rolePermissionService.DeleteRoleAsync(id);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("permissions")]
    [Permission("Role.View")]
    public async Task<IActionResult> GetPermissions([FromQuery] PermissionListRequestModel request)
    {
        _logger.LogInformation("GetPermissions => Fetching permissions");
        var response = await _rolePermissionService.GetPermissionsAsync(request);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("assign")]
    [Permission("Role.Update")]
    public async Task<IActionResult> AssignPermissions([FromBody] AssignRolePermissionsRequestModel request)
    {
        _logger.LogInformation("AssignPermissions => Assigning permissions to role id: {RoleId}", request.RoleId);
        var response = await _rolePermissionService.AssignPermissionsAsync(request);
        if (!response.isSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
