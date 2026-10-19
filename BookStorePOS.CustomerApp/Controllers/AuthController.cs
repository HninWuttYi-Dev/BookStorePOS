using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Auth;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        [ActionName("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequestModel requestModel)
        {
            try
            {
                string rawIdentifier = !string.IsNullOrWhiteSpace(requestModel.Phone)
                    ? requestModel.Phone
                    : (requestModel.Username ?? string.Empty);

                _logger.LogInformation("Customer Login => Attempting login for identifier: {Identifier}", rawIdentifier);

                if (string.IsNullOrWhiteSpace(rawIdentifier) || string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    return Json(new LoginResponseModel
                    {
                        IsSuccess = false,
                        Message = "Phone number and password are required."
                    });
                }

                if (string.IsNullOrWhiteSpace(requestModel.Username) && !string.IsNullOrWhiteSpace(requestModel.Phone))
                {
                    requestModel.Username = requestModel.Phone;
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/login", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<LoginResponseModel>(jsonString)
                    ?? new LoginResponseModel { IsSuccess = false, Message = "Unexpected server response." };

                if (model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    string displayUser = !string.IsNullOrWhiteSpace(model.FullName) 
                        ? model.FullName 
                        : (!string.IsNullOrWhiteSpace(model.Username) ? model.Username : rawIdentifier);

                    try { displayUser = Uri.UnescapeDataString(displayUser); } catch { }

                    _logger.LogInformation("Customer Login => Login successful for: {DisplayUser}", displayUser);

                    Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    });

                    if (!string.IsNullOrEmpty(model.RefreshToken))
                    {
                        Response.Cookies.Append("customer_refresh", model.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }

                    Response.Cookies.Append("customer_user", Uri.EscapeDataString(displayUser), new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                }
                else
                {
                    _logger.LogWarning("Customer Login => Login failed: {Message}", model.Message);
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer Login => Exception occurred");
                return StatusCode(500, new LoginResponseModel
                {
                    IsSuccess = false,
                    Message = "Unable to connect to authentication service: " + ex.Message
                });
            }
        }

        [HttpGet]
        [ActionName("Register")]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation("Customer Register => Attempting registration for username: {Username}", requestModel.Username);

                if (string.IsNullOrWhiteSpace(requestModel.Username) ||
                    string.IsNullOrWhiteSpace(requestModel.FullName) ||
                    string.IsNullOrWhiteSpace(requestModel.Phone) ||
                    string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    return Json(new RegisterResponseModel
                    {
                        IsSuccess = false,
                        Message = "Full name, phone number, username, and password are required."
                    });
                }

                if (requestModel.Username.Contains(' ') || requestModel.Username.Any(char.IsWhiteSpace))
                {
                    return Json(new RegisterResponseModel
                    {
                        IsSuccess = false,
                        Message = "Username cannot contain spaces."
                    });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/register", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RegisterResponseModel>(jsonString)
                    ?? new RegisterResponseModel { IsSuccess = false, Message = "Unexpected server response." };

                if (model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    string displayUser = !string.IsNullOrWhiteSpace(model.FullName)
                        ? model.FullName
                        : (!string.IsNullOrWhiteSpace(requestModel.FullName)
                            ? requestModel.FullName
                            : (!string.IsNullOrWhiteSpace(model.Username) ? model.Username : requestModel.Username));

                    try { displayUser = Uri.UnescapeDataString(displayUser); } catch { }

                    _logger.LogInformation("Customer Register => Registration successful for: {DisplayUser}", displayUser);

                    Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    });

                    if (!string.IsNullOrEmpty(model.RefreshToken))
                    {
                        Response.Cookies.Append("customer_refresh", model.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }

                    Response.Cookies.Append("customer_user", Uri.EscapeDataString(displayUser), new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                }
                else
                {
                    _logger.LogWarning("Customer Register => Registration failed: {Message}", model.Message);
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer Register => Exception occurred");
                return StatusCode(500, new RegisterResponseModel
                {
                    IsSuccess = false,
                    Message = "Unable to connect to registration service: " + ex.Message
                });
            }
        }

        [HttpPost]
        [ActionName("Refresh")]
        public async Task<IActionResult> RefreshAsync()
        {
            try
            {
                var refreshToken = Request.Cookies["customer_refresh"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Json(new RefreshTokenResponseModel
                    {
                        IsSuccess = false,
                        Message = "No refresh token found in cookies."
                    });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                var request = new RefreshTokenRequestModel { RefreshToken = refreshToken };
                var json = JsonConvert.SerializeObject(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/refresh", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RefreshTokenResponseModel>(jsonString)
                    ?? new RefreshTokenResponseModel { IsSuccess = false, Message = "Unexpected server response." };

                if (model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    });

                    if (!string.IsNullOrEmpty(model.RefreshToken))
                    {
                        Response.Cookies.Append("customer_refresh", model.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }

                    _logger.LogInformation("Customer Refresh => Token refreshed successfully for: {Username}", model.Username);
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer Refresh => Exception occurred");
                return StatusCode(500, new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Message = "Unable to connect to refresh service: " + ex.Message
                });
            }
        }

        [HttpGet]
        [ActionName("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["customer_refresh"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var client = _httpClientFactory.CreateClient("WebAPI");
                    var logoutModel = new LogoutRequestModel { RefreshToken = refreshToken };
                    var json = JsonConvert.SerializeObject(logoutModel);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await client.PostAsync("api/Auth/logout", content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Customer Logout => Error revoking refresh token on WebAPI");
            }

            Response.Cookies.Delete("customer_token");
            Response.Cookies.Delete("customer_refresh");
            Response.Cookies.Delete("customer_user");
            return RedirectToAction("Index", "Books");
        }
    }
}
