using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.Domain.Features.Auth;
using BookStorePOS.Shared.Models.Auth;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
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
        _logger.LogInformation("Login => Received login request for username: {Username}", request.Username);
        var response = await _authService.LoginAsync(request);

        if (!response.isSuccess)
        {
            _logger.LogWarning("Login => Invalid login credentials for username: {Username}", request.Username);
            return Unauthorized(response);
        }

        _logger.LogInformation("Login => Login succeeded for username: {Username}", request.Username);
        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
    {
        _logger.LogInformation("Register => Received registration request for username: {Username}", request.Username);
        var response = await _authService.RegisterCustomerAsync(request);

        if (!response.isSuccess)
        {
            _logger.LogWarning("Register => Registration failed for username: {Username}. Reason: {Message}", request.Username, response.Message);
            return BadRequest(response);
        }

        _logger.LogInformation("Register => Customer registration succeeded for username: {Username}", request.Username);
        return Ok(response);
    }
}
