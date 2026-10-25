using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BookStorePOS.MvcApp.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthHeaderHandler> _logger;

    public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthHeaderHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        var path = request.RequestUri?.AbsolutePath ?? "";

        // Don't auto-refresh when calling auth endpoints
        bool isAuthEndpoint = path.Contains("refresh", StringComparison.OrdinalIgnoreCase) || 
                             path.Contains("login", StringComparison.OrdinalIgnoreCase);

        if (context != null && !isAuthEndpoint)
        {
            // 1. Get current token (from request items or cookies)
            var token = context.Items["pos_token"] as string ?? context.Request.Cookies["pos_token"];

            // 2. If token is missing, but pos_refresh cookie exists, auto-refresh it
            if (string.IsNullOrEmpty(token))
            {
                var refreshToken = context.Request.Cookies["pos_refresh"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("AuthHeaderHandler => pos_token missing, auto-refreshing token...");
                    token = await RefreshTokenAsync(context, refreshToken, cancellationToken);
                }
            }

            // 3. Attach Bearer token to request
            if (!string.IsNullOrEmpty(token) && request.Headers.Authorization == null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AuthHeaderHandler => Unable to connect to WebAPI: {Message}", ex.Message);
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent(
                    $"{{\"isSuccess\":false,\"message\":\"POS WebAPI service is unavailable: {ex.Message}\"}}",
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }

    private async Task<string?> RefreshTokenAsync(HttpContext context, string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/refresh")
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new RefreshTokenRequestModel { RefreshToken = refreshToken }),
                    Encoding.UTF8,
                    "application/json")
            };

            var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);
            if (!refreshResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("AuthHeaderHandler => Refresh failed with status {StatusCode}", refreshResponse.StatusCode);
                return null;
            }

            var jsonString = await refreshResponse.Content.ReadAsStringAsync(cancellationToken);
            var model = JsonConvert.DeserializeObject<RefreshTokenResponseModel>(jsonString);

            if (model != null && model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
            {
                _logger.LogInformation("AuthHeaderHandler => pos_token refreshed successfully");

                // Save token for any other API calls in this same page request
                context.Items["pos_token"] = model.AccessToken;

                // Save new token to browser cookies
                context.Response.Cookies.Append("pos_token", model.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                if (!string.IsNullOrEmpty(model.RefreshToken))
                {
                    context.Response.Cookies.Append("pos_refresh", model.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                }

                return model.AccessToken;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthHeaderHandler => Exception during token refresh: {Message}", ex.Message);
            return null;
        }
    }
}
