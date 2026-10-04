using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using BookStorePOS.Shared.Models.Edition;

namespace BookStorePOS.MvcApp.Controllers
{
    public class EditionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EditionController> _logger;

        public EditionController(IHttpClientFactory httpClientFactory, ILogger<EditionController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> EditionListAsync(EditionListRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation("Edition List Async => Fetching editions");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&EditionName={Uri.EscapeDataString(requestModel.EditionName ?? "")}";
                var httpResponse = await client.GetAsync($"api/edition{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<EditionListResponseModel>(jsonString);

                if (response != null && response.isSuccess && response.Data != null)
                {
                    ViewData["Editions"] = response.Data;
                    ViewData["CurrentPage"] = response.Page;
                    ViewData["TotalPages"] = response.TotalPages;
                    ViewData["EditionNameFilter"] = requestModel.EditionName;
                    _logger.LogInformation("Edition List Async => Editions fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"Edition List Async => Failed to fetch editions: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                }

                return View("EditionList");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionListAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> EditionSearchAsync(string query)
        {
        try
        {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page=1&Limit=10&EditionName={Uri.EscapeDataString(query ?? "")}";
                var httpResponse = await client.GetAsync($"api/edition{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<EditionListResponseModel>(jsonString);
                if (response != null && response.isSuccess && response.Data != null)
                {
                    return Json(response.Data);
                }
                return Json(new List<EditionModel>());
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionSearchAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("Create")]
        public IActionResult EditionCreate()
        {
        try
        {
                return View("EditionCreate");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionCreate => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> EditionSaveAsync(EditionCreateRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation("Edition Save Async => Creating new edition");

                if (string.IsNullOrWhiteSpace(requestModel.EditionName))
                {
                    _logger.LogWarning("Edition Save Async => Edition name is required");
                    return Json(new EditionCreateResponseModel { isSuccess = false, Message = "Edition name is required" });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
                var httpResponse = await client.PostAsync("api/edition", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<EditionCreateResponseModel>(jsonString) ?? new EditionCreateResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Edition Save Async => Edition created successfully");
                    TempData["Message"] = model.Message ?? "Edition created successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Edition Save Async => Failed to create edition: {model?.Message}");
                }

                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionSaveAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("Edit")]
        public async Task<IActionResult> EditionEditAsync(int id)
        {
        try
        {
                _logger.LogInformation($"Edition Edit Async => Fetching edition {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/edition/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<EditionByIdResponseModel>(jsonString);
            
                if (model == null || !model.isSuccess || model.Data == null)
                {
                    _logger.LogWarning($"Edition Edit Async => Failed to fetch edition {id}: {model?.Message}");
                    TempData["isSuccess"] = false;
                    TempData["Message"] = model?.Message ?? "Edition not found.";
                    return Redirect("/Edition");
                }

                ViewData["Id"] = model.Data.EditionId;
                ViewData["EditionName"] = model.Data.EditionName;

                return View("EditionEdit", model.Data);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionEditAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> EditionUpdateAsync(int id, EditionPatchRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation($"Edition Update Async => Updating edition {id}");
                requestModel.EditionId = id;

                if (string.IsNullOrWhiteSpace(requestModel.EditionName))
                {
                    return Json(new EditionPatchResponseModel { isSuccess = false, Message = "Edition name is required." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                string json = JsonConvert.SerializeObject(requestModel);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
                var httpResponse = await client.PatchAsync($"api/edition/{id}", stringContent);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<EditionPatchResponseModel>(jsonString) ?? new EditionPatchResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Edition Update Async => Edition updated successfully");
                    TempData["Message"] = model.Message ?? "Edition updated successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Edition Update Async => Failed to update edition: {model?.Message}");
                }

                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionUpdateAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> EditionDeleteAsync(EditionDeleteRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation($"Edition Delete Async => Deleting edition {requestModel.EditionId}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.DeleteAsync($"api/edition/{requestModel.EditionId}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<EditionDeleteResponseModel>(jsonString) ?? new EditionDeleteResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Edition Delete Async => Edition deleted successfully");
                    TempData["Message"] = model.Message ?? "Edition deleted successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Edition Delete Async => Failed to delete edition: {model?.Message}");
                }
                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditionDeleteAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
    }
}
