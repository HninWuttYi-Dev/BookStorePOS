using System;
using System.Collections.Generic;
using System.IO;
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

            // 2. Proactive check: If token is missing OR expired, and pos_refresh exists, auto-refresh it
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                var refreshToken = context.Request.Cookies["pos_refresh"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("AuthHeaderHandler => pos_token is missing or expired, auto-refreshing token...");
                    token = await RefreshTokenAsync(context, refreshToken, cancellationToken);
                }
            }

            // 3. Attach Bearer token to request
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        HttpResponseMessage response;
        try
        {
            response = await base.SendAsync(request, cancellationToken);
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

        // 4. Reactive recovery: If response is 401 Unauthorized, try one fresh token refresh and retry
        if (response.StatusCode == HttpStatusCode.Unauthorized && context != null && !isAuthEndpoint)
        {
            var refreshToken = context.Request.Cookies["pos_refresh"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogInformation("AuthHeaderHandler => Received 401 Unauthorized, attempting recovery refresh...");
                var recoveredToken = await RefreshTokenAsync(context, refreshToken, cancellationToken);
                if (!string.IsNullOrEmpty(recoveredToken))
                {
                    try
                    {
                        var retryRequest = await CloneHttpRequestMessageAsync(request);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", recoveredToken);
                        response.Dispose();
                        return await base.SendAsync(retryRequest, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "AuthHeaderHandler => Retry after token refresh failed: {Message}", ex.Message);
                    }
                }
            }
        }

        return response;
    }

    private async Task<string?> RefreshTokenAsync(HttpContext context, string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5201/api/Auth/refresh")
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
                ClearAuthCookies(context);
                return null;
            }

            var jsonString = await refreshResponse.Content.ReadAsStringAsync(cancellationToken);
            var model = JsonConvert.DeserializeObject<RefreshTokenResponseModel>(jsonString);

            if (model != null && model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
            {
                _logger.LogInformation("AuthHeaderHandler => pos_token refreshed successfully");

                // Save token for any other API calls in this same page request
                context.Items["pos_token"] = model.AccessToken;

                // Save new token to browser cookies - Set 7-day cookie expiration matching pos_refresh
                // (The JWT itself contains its cryptographic expiration; cookie lifespan prevents premature browser deletion)
                context.Response.Cookies.Append("pos_token", model.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
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

            ClearAuthCookies(context);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthHeaderHandler => Exception during token refresh: {Message}", ex.Message);
            return null;
        }
    }

    private void ClearAuthCookies(HttpContext context)
    {
        try
        {
            context.Response.Cookies.Delete("pos_token");
            context.Response.Cookies.Delete("pos_refresh");
            context.Response.Cookies.Delete("pos_user");
            context.Response.Cookies.Delete("pos_role");
            context.Response.Cookies.Delete("pos_permissions");
            context.Items["Auth_Expired"] = true;
        }
        catch
        {
            // Ignore response header modification exceptions if response has already started
        }
    }

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);

            var payloadObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (payloadObj != null && payloadObj.TryGetValue("exp", out var expObj))
            {
                var expSeconds = Convert.ToInt64(expObj);
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                // Proactively consider expired if within 30 seconds of expiry
                return expTime <= DateTimeOffset.UtcNow.AddSeconds(30);
            }
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);
        if (req.Content != null)
        {
            var ms = new MemoryStream();
            await req.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            if (req.Content.Headers.ContentType != null)
            {
                clone.Content.Headers.ContentType = req.Content.Headers.ContentType;
            }
        }
        foreach (var header in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return clone;
    }
}
