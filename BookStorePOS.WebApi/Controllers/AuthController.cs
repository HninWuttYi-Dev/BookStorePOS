using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Domain.Features.Auth;
using BookStorePOS.Shared.Models.Auth;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        _logger.LogInformation("Login => Received login request for identifier: {Identifier}", !string.IsNullOrWhiteSpace(request.Phone) ? request.Phone : request.Username);
        var response = await _authService.LoginAsync(request);
        return ExecuteResult(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
    {
        _logger.LogInformation("Register => Received registration request for username: {Username}", request.Username);
        var response = await _authService.RegisterCustomerAsync(request);
        return ExecuteResult(response);
    }
}
