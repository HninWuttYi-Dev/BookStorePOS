using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace BookStorePOS.MvcApp.Controllers
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
            ViewData["ReturnUrl"] = returnUrl ?? "/Dashboard";
            return View("Login");
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequestModel requestModel)
        {
            try
            {
                // Admin/Staff Portal strictly requires Phone number (usernames are not allowed)
                if (string.IsNullOrWhiteSpace(requestModel.Phone) || string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    return Json(new LoginResponseModel
                    {
                        IsSuccess = false,
                        Message = "Phone number and password are required."
                    });
                }

                string normalizedPhone = NormalizeMyanmarPhone(requestModel.Phone);
                _logger.LogInformation("Staff Login => Attempting staff login with phone: {Phone}", normalizedPhone);

                if (!Regex.IsMatch(normalizedPhone, @"^09\d{7,9}$"))
                {
                    return Json(new LoginResponseModel
                    {
                        IsSuccess = false,
                        Message = "A valid Myanmar phone number is required (e.g. 09789456123)."
                    });
                }

                // Strictly authenticate using Phone
                var apiRequest = new LoginRequestModel
                {
                    Phone = normalizedPhone,
                    Username = null,
                    Password = requestModel.Password
                };

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(apiRequest);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/login", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<LoginResponseModel>(jsonString)
                    ?? new LoginResponseModel { IsSuccess = false, Message = "Unexpected server response." };

                if (model.IsSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    if (string.Equals(model.Role, "Customer", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Staff Login => Customer attempted to access Staff Portal: {Phone}", normalizedPhone);
                        return Json(new LoginResponseModel
                        {
                            IsSuccess = false,
                            Message = "Access denied. Customer accounts cannot access the Staff POS Portal."
                        });
                    }

                    string displayUser = !string.IsNullOrWhiteSpace(model.FullName) 
                        ? model.FullName 
                        : (!string.IsNullOrWhiteSpace(model.Username) ? model.Username : normalizedPhone);

                    _logger.LogInformation("Staff Login => Staff login succeeded for: {DisplayUser}, Role: {Role}", displayUser, model.Role);

                    Response.Cookies.Append("pos_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    });

                    if (!string.IsNullOrEmpty(model.RefreshToken))
                    {
                        Response.Cookies.Append("pos_refresh", model.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }

                    Response.Cookies.Append("pos_user", Uri.EscapeDataString(displayUser), new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    Response.Cookies.Append("pos_role", model.Role ?? "Staff", new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                }
                else
                {
                    _logger.LogWarning("Staff Login => Staff login failed: {Message}", model.Message);
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Staff Login => Exception occurred");
                return StatusCode(500, new LoginResponseModel
                {
                    IsSuccess = false,
                    Message = "Unable to connect to POS Auth service: " + ex.Message
                });
            }
        }

        [HttpGet]
        [ActionName("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["pos_refresh"];
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
                _logger.LogWarning(ex, "Staff Logout => Error revoking refresh token on WebAPI");
            }

            Response.Cookies.Delete("pos_token");
            Response.Cookies.Delete("pos_refresh");
            Response.Cookies.Delete("pos_user");
            Response.Cookies.Delete("pos_role");
            return RedirectToAction("Login", "Auth");
        }

        private static string NormalizeMyanmarPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            string p = phone.Trim().Replace(" ", "").Replace("-", "");
            if (p.StartsWith("+959"))
                p = "09" + p.Substring(4);
            else if (p.StartsWith("959"))
                p = "09" + p.Substring(3);
            else if (p.StartsWith("9") && p.Length == 10)
                p = "0" + p;
            else if (p.Length == 9 && !p.StartsWith("0"))
                p = "09" + p;
            return p;
        }
    }
}
