using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.RolePermission;

namespace BookStorePOS.MvcApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IHttpClientFactory httpClientFactory, ILogger<RoleController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> RoleListAsync(RoleListRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation("Role List Async => Fetching roles list");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var page = requestModel.PageNumber > 0 ? requestModel.PageNumber : 1;
                var size = requestModel.PageSize > 0 ? requestModel.PageSize : 10;
                var qs = $"?PageNumber={page}&PageSize={size}&RoleName={Uri.EscapeDataString(requestModel.RoleName ?? "")}";

                var httpResponse = await client.GetAsync($"api/RolePermission/roles{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RoleListResponseModel>(jsonString);

                if (response != null && response.IsSuccess && response.Data != null)
                {
                    ViewData["Roles"] = response.Data;
                    ViewData["CurrentPage"] = response.PageNumber;
                    ViewData["TotalCount"] = response.TotalCount;
                    ViewData["PageSize"] = response.PageSize;
                    var totalPages = (int)Math.Ceiling((double)response.TotalCount / response.PageSize);
                    ViewData["TotalPages"] = totalPages > 0 ? totalPages : 1;
                    ViewData["RoleNameFilter"] = requestModel.RoleName;
                    _logger.LogInformation("Role List Async => Roles fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"Role List Async => Failed to fetch roles: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                }

                return View("RoleList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleListAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Create")]
        public async Task<IActionResult> RoleCreateAsync()
        {
            try
            {
                _logger.LogInformation("Role Create Async => Fetching permissions for role creation");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var permResponse = await client.GetAsync("api/RolePermission/permissions?PageNumber=1&PageSize=100");
                var permJson = await permResponse.Content.ReadAsStringAsync();
                var permissionsModel = JsonConvert.DeserializeObject<PermissionListResponseModel>(permJson);

                var allPermissions = permissionsModel?.Data ?? new List<PermissionModel>();
                ViewData["AllPermissions"] = allPermissions;

                return View("RoleCreate");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleCreateAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> RoleSaveAsync([FromBody] RoleCreateRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation("Role Save Async => Creating new role");

                if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.RoleName))
                {
                    _logger.LogWarning("Role Save Async => Role name is required");
                    return Json(new RoleCreateResponseModel { IsSuccess = false, Message = "Role name is required." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);

                var httpResponse = await client.PostAsync("api/RolePermission/roles", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RoleCreateResponseModel>(jsonString)
                    ?? new RoleCreateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("Role Save Async => Role created successfully");
                    TempData["Message"] = model.Message ?? "Role created successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Role Save Async => Failed to create role: {model?.Message}");
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleSaveAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [ActionName("Edit")]
        public async Task<IActionResult> RoleEditAsync(int id)
        {
            try
            {
                if (id <= 0 || !ModelState.IsValid)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Role Edit Async => Fetching role details for RoleId: {id}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var roleResponse = await client.GetAsync($"api/RolePermission/roles/{id}");

                if (roleResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var roleJson = await roleResponse.Content.ReadAsStringAsync();
                var roleModel = JsonConvert.DeserializeObject<RoleByIdResponseModel>(roleJson);

                if (roleModel == null || !roleModel.IsSuccess || roleModel.Data == null || roleModel.Type == EnumResType.NotFound)
                {
                    _logger.LogWarning($"Role Edit Async => Failed to fetch role {id}: {roleModel?.Message}");
                    return NotFound();
                }

                var permResponse = await client.GetAsync("api/RolePermission/permissions?PageNumber=1&PageSize=100");
                var permJson = await permResponse.Content.ReadAsStringAsync();
                var permissionsModel = JsonConvert.DeserializeObject<PermissionListResponseModel>(permJson);

                var allPermissions = permissionsModel?.Data ?? new List<PermissionModel>();
                ViewData["AllPermissions"] = allPermissions;
                ViewData["AssignedPermissionIds"] = roleModel.Data.Permissions.Select(p => p.PermissionId).ToList();

                _logger.LogInformation($"Role Edit Async => Role details fetched successfully for RoleId: {id}");
                return View("RoleEdit", roleModel.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleEditAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> RoleUpdateAsync(int id, [FromBody] RoleUpdateRequestModel requestModel)
        {
            try
            {
                _logger.LogInformation($"Role Update Async => Updating role details for RoleId: {id}");

                if (requestModel == null || string.IsNullOrWhiteSpace(requestModel.RoleName))
                {
                    return Json(new RoleUpdateResponseModel { IsSuccess = false, Message = "Role name is required." });
                }

                requestModel.RoleId = id;
                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);

                var httpResponse = await client.PutAsync($"api/RolePermission/roles/{id}", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RoleUpdateResponseModel>(jsonString)
                    ?? new RoleUpdateResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("Role Update Async => Role updated successfully");
                    TempData["Message"] = model.Message ?? "Role updated successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Role Update Async => Failed to update role: {model?.Message}");
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleUpdateAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> RoleDeleteAsync(RoleDeleteRequestModel requestModel, int? id = null)
        {
            try
            {
                var roleId = requestModel?.RoleId > 0 ? requestModel.RoleId : (id ?? 0);
                _logger.LogInformation($"Role Delete Async => Deleting role {roleId}");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.DeleteAsync($"api/RolePermission/roles/{roleId}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<RoleDeleteResponseModel>(jsonString)
                    ?? new RoleDeleteResponseModel { IsSuccess = false, Message = "Unknown error" };

                if (model != null && model.IsSuccess)
                {
                    _logger.LogInformation("Role Delete Async => Role deleted successfully");
                    TempData["Message"] = model.Message ?? "Role deleted successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Role Delete Async => Failed to delete role: {model?.Message}");
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleDeleteAsync => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
