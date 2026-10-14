using Microsoft.AspNetCore.Mvc;
using System.Text;

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
                _logger.LogInformation("Customer Login => Attempting login for username: {Username}", requestModel.Username);

                if (string.IsNullOrWhiteSpace(requestModel.Username) || string.IsNullOrWhiteSpace(requestModel.Password))
                {
                    return Json(new LoginResponseModel
                    {
                        isSuccess = false,
                        Message = "Username and password are required."
                    });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/login", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<LoginResponseModel>(jsonString)
                    ?? new LoginResponseModel { isSuccess = false, Message = "Unexpected server response." };

                if (model.isSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    _logger.LogInformation("Customer Login => Login successful for username: {Username}", requestModel.Username);

                    Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    Response.Cookies.Append("customer_user", requestModel.Username, new CookieOptions
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
                    isSuccess = false,
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
                        isSuccess = false,
                        Message = "Full name, phone number, username, and password are required."
                    });
                }

                if (requestModel.Username.Contains(' ') || !System.Text.RegularExpressions.Regex.IsMatch(requestModel.Username.Trim(), @"^[a-zA-Z0-9_.-]+$"))
                {
                    return Json(new RegisterResponseModel
                    {
                        isSuccess = false,
                        Message = "Username cannot contain spaces or special characters. Please use letters, numbers, '.', '_', or '-' only."
                    });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync("api/Auth/register", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RegisterResponseModel>(jsonString)
                    ?? new RegisterResponseModel { isSuccess = false, Message = "Unexpected server response." };

                if (model.isSuccess && !string.IsNullOrEmpty(model.AccessToken))
                {
                    _logger.LogInformation("Customer Register => Registration successful for username: {Username}", requestModel.Username);

                    Response.Cookies.Append("customer_token", model.AccessToken, new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    Response.Cookies.Append("customer_user", requestModel.Username, new CookieOptions
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
                    isSuccess = false,
                    Message = "Unable to connect to registration service: " + ex.Message
                });
            }
        }

        [HttpGet]
        [ActionName("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("customer_token");
            Response.Cookies.Delete("customer_user");
            return RedirectToAction("Index", "Books");
        }
    }
}
