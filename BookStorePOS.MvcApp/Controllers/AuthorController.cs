using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Author;

namespace BookStorePOS.MvcApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IHttpClientFactory httpClientFactory, ILogger<AuthorController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> AuthorListAsync(AuthorListRequestModel requestModel)
        {
            _logger.LogInformation("Author List Async => Fetching authors");

            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&AuthorName={Uri.EscapeDataString(requestModel.AuthorName ?? "")}";
            var httpResponse = await client.GetAsync($"api/author{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<AuthorListResponseModel>(jsonString);

            if (response != null && response.isSuccess && response.Data != null)
            {
                ViewData["Authors"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["AuthorNameFilter"] = requestModel.AuthorName;
                _logger.LogInformation("Author List Async => Authors fetched successfully");
            }
            else
            {
                _logger.LogWarning($"Author List Async => Failed to fetch authors: {response?.Message}");
                TempData["Message"] = response?.Message;
                TempData["isSuccess"] = false;
            }

            return View("AuthorList");
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> AuthorSearchAsync(string query)
        {
            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page=1&Limit=10&AuthorName={Uri.EscapeDataString(query ?? "")}";
            var httpResponse = await client.GetAsync($"api/author{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<AuthorListResponseModel>(jsonString);
            if (response != null && response.isSuccess && response.Data != null)
            {
                return Json(response.Data);
            }
            return Json(new List<AuthorModel>());
        }

        [ActionName("Create")]
        public IActionResult AuthorCreate()
        {
            return View("AuthorCreate");
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> AuthorSaveAsync(AuthorCreateRequestModel requestModel)
        {
            _logger.LogInformation("Author Save Async => Creating new author");

            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                _logger.LogWarning("Author Save Async => Author name is required");
                return Json(new AuthorCreateResponseModel { isSuccess = false, Message = "Author name is required" });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            var content = new StringContent(JsonConvert.SerializeObject(requestModel), System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync("api/author", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AuthorCreateResponseModel>(jsonString) ?? new AuthorCreateResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Author Save Async => Author created successfully");
                TempData["Message"] = model.Message ?? "Author created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Save Async => Failed to create author: {model?.Message}");
            }

            return Json(model);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> AuthorEditAsync(int id)
        {
            _logger.LogInformation($"Author Edit Async => Fetching author {id}");
            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.GetAsync($"api/author/{id}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AuthorByIdResponseModel>(jsonString);
            
            if (model == null || !model.isSuccess || model.Data == null)
            {
                _logger.LogWarning($"Author Edit Async => Failed to fetch author {id}: {model?.Message}");
                TempData["isSuccess"] = false;
                TempData["Message"] = model?.Message ?? "Author not found.";
                return Redirect("/Author");
            }

            ViewData["Id"] = model.Data.AuthorId;
            ViewData["AuthorName"] = model.Data.AuthorName;

            return View("AuthorEdit", model.Data);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> AuthorUpdateAsync(int id, AuthorPatchRequestModel requestModel)
        {
            _logger.LogInformation($"Author Update Async => Updating author {id}");
            requestModel.AuthorId = id;

            if (string.IsNullOrWhiteSpace(requestModel.AuthorName))
            {
                return Json(new AuthorPatchResponseModel { isSuccess = false, Message = "Author name is required." });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            var content = new StringContent(JsonConvert.SerializeObject(requestModel), System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await client.PatchAsync($"api/author/{id}", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AuthorPatchResponseModel>(jsonString) ?? new AuthorPatchResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Author Update Async => Author updated successfully");
                TempData["Message"] = model.Message ?? "Author updated successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Update Async => Failed to update author: {model?.Message}");
            }

            return Json(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> AuthorDeleteAsync(AuthorDeleteRequestModel requestModel)
        {
            _logger.LogInformation($"Author Delete Async => Deleting author {requestModel.AuthorId}");
            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.DeleteAsync($"api/author/{requestModel.AuthorId}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AuthorDeleteResponseModel>(jsonString) ?? new AuthorDeleteResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Author Delete Async => Author deleted successfully");
                TempData["Message"] = model.Message ?? "Author deleted successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Delete Async => Failed to delete author: {model?.Message}");
            }
            return Json(model);
        }
    }
}
