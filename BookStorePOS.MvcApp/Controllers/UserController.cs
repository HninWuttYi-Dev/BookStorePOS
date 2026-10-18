using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Net;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.User;
using BookStorePOS.Shared.Models.RolePermission;

namespace BookStorePOS.MvcApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(IHttpClientFactory httpClientFactory, ILogger<UserController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> UserListAsync(UserListRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation("User List Async => Fetching users");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var page = requestModel.Page > 0 ? requestModel.Page : 1;
                var limit = requestModel.Limit > 0 ? requestModel.Limit : 10;
                var qs = $"?Page={page}&Limit={limit}&Search={Uri.EscapeDataString(requestModel.Search ?? "")}";
                if (requestModel.RoleId.HasValue && requestModel.RoleId.Value > 0)
                {
                    qs += $"&RoleId={requestModel.RoleId.Value}";
                }

                var httpResponse = await client.GetAsync($"api/user{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<UserListResponseModel>(jsonString);

                // Fetch roles for filter dropdown
                var rolesResponse = await client.GetAsync("api/RolePermission/roles?PageSize=100");
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    var rolesResult = JsonConvert.DeserializeObject<RoleListResponseModel>(rolesJson);
                    var activeRoles = rolesResult?.Data?
                        .Where(r => !r.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                        .ToList() ?? new List<RoleModel>();
                    ViewData["Roles"] = activeRoles;
                }

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    ViewData["Users"] = response.Data;
                    ViewData["CurrentPage"] = response.Page > 0 ? response.Page : page;
                    ViewData["PageSize"] = response.Limit > 0 ? response.Limit : limit;
                    ViewData["TotalCount"] = response.Count;
                    ViewData["TotalPages"] = response.TotalPages > 0 ? response.TotalPages : 1;
                    ViewData["SearchFilter"] = requestModel.Search ?? string.Empty;
                    ViewData["SelectedRoleId"] = requestModel.RoleId;
                    _logger.LogInformation("User List Async => Users fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"User List Async => Failed to fetch users: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                }

                return View("UserList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserListAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Create")]
        public async Task<IActionResult> UserCreateAsync()
        {
            try
            {
                _logger.LogInformation("User Create Async => Loading user create form");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var rolesResponse = await client.GetAsync("api/RolePermission/roles?PageSize=100");
                var activeRoles = new List<RoleModel>();

                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    var rolesResult = JsonConvert.DeserializeObject<RoleListResponseModel>(rolesJson);
                    activeRoles = rolesResult?.Data?
                        .Where(r => !r.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                        .ToList() ?? new List<RoleModel>();
                }

                ViewData["Roles"] = activeRoles;
                return View("UserCreate");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserCreateAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> UserSaveAsync([FromBody] UserCreateRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation("User Save Async => Creating new staff user");

                if (requestModel == null)
                {
                    _logger.LogWarning("User Save Async => No user data provided");
                    return Json(new UserCreateResponseModel { IsSuccess = false, Message = "Invalid staff data provided." });
                }

                if (string.IsNullOrWhiteSpace(requestModel.Username))
                {
                    return Json(new UserCreateResponseModel { IsSuccess = false, Message = "Login username is required." });
                }

                if (requestModel.Username.Contains(' ') || requestModel.Username.Any(char.IsWhiteSpace))
                {
                    return Json(new UserCreateResponseModel { IsSuccess = false, Message = "Username cannot contain spaces." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
                var httpResponse = await client.PostAsync("api/user", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<UserCreateResponseModel>(jsonString) 
                    ?? new UserCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation("User Save Async => User created successfully");
                    TempData["Message"] = response.Message ?? "Staff account created successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"User Save Async => Failed to create user: {response?.Message}");
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserSaveAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Edit")]
        public async Task<IActionResult> UserEditAsync(int id)
        {
            try
            {
                if (id <= 0 || !ModelState.IsValid)
                {
                    return NotFound();
                }

                _logger.LogInformation($"User Edit Async => Fetching user {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var userResponse = await client.GetAsync($"api/user/{id}");

                if (userResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserGetByIdResponseModel>(userJson);

                if (model == null || !model.IsSuccess || model.Data == null || model.Type == EnumResType.NotFound)
                {
                    _logger.LogWarning($"User Edit Async => Failed to fetch user {id}: {model?.Message}");
                    return NotFound();
                }

                var rolesResponse = await client.GetAsync("api/RolePermission/roles?PageSize=100");
                var activeRoles = new List<RoleModel>();
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    var rolesResult = JsonConvert.DeserializeObject<RoleListResponseModel>(rolesJson);
                    activeRoles = rolesResult?.Data?
                        .Where(r => !r.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                        .ToList() ?? new List<RoleModel>();
                }

                ViewData["Roles"] = activeRoles;
                _logger.LogInformation($"User Edit Async => User {id} fetched successfully");
                return View("UserEdit", model.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserEditAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> UserUpdateAsync(int id, [FromBody] UserUpdateRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation($"User Update Async => Updating user {id}");

                if (requestModel == null)
                {
                    return Json(new UserUpdateResponseModel { IsSuccess = false, Message = "Invalid staff update data." });
                }

                requestModel.UserId = id;

                if (string.IsNullOrWhiteSpace(requestModel.Username))
                {
                    return Json(new UserUpdateResponseModel { IsSuccess = false, Message = "Login username is required." });
                }

                if (requestModel.Username.Contains(' ') || requestModel.Username.Any(char.IsWhiteSpace))
                {
                    return Json(new UserUpdateResponseModel { IsSuccess = false, Message = "Username cannot contain spaces." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
                var httpResponse = await client.PatchAsync($"api/user/{id}", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<UserUpdateResponseModel>(jsonString) 
                    ?? new UserUpdateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation("User Update Async => User updated successfully");
                    TempData["Message"] = response.Message ?? "Staff account updated successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"User Update Async => Failed to update user: {response?.Message}");
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserUpdateAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> UserDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"User Delete Async => Deleting user {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.DeleteAsync($"api/user/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<UserDeleteResponseModel>(jsonString) 
                    ?? new UserDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (response != null && response.IsSuccess)
                {
                    _logger.LogInformation("User Delete Async => User deleted successfully");
                    TempData["Message"] = response.Message ?? "Staff account deleted successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"User Delete Async => Failed to delete user: {response?.Message}");
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserDeleteAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
