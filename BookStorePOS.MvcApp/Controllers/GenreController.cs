using Microsoft.AspNetCore.Mvc;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
namespace BookStorePOS.MvcApp.Controllers
{
    public class GenreController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IHttpClientFactory httpClientFactory, ILogger<GenreController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> GenreListAsync(GenreListRequestModel requestModel)
        {
            _logger.LogInformation("Genre List Async => Fetching genres");

            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&GenreName={Uri.EscapeDataString(requestModel.GenreName ?? "")}";
            var httpResponse = await client.GetAsync($"api/genre{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GenreListResponseModel>(jsonString);

            if (response != null && response.isSuccess && response.Data != null)
            {
                ViewData["Genres"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["GenreNameFilter"] = requestModel.GenreName;
                _logger.LogInformation("Genre List Async => Genres fetched successfully");
            }
            else
            {
                _logger.LogWarning($"Genre List Async => Failed to fetch genres: {response?.Message}");
                TempData["Message"] = response?.Message;
                TempData["isSuccess"] = false;
            }

            return View("GenreList");
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> GenreSearchAsync(string query)
        {
            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page=1&Limit=10&GenreName={Uri.EscapeDataString(query ?? "")}";
            var httpResponse = await client.GetAsync($"api/genre{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GenreListResponseModel>(jsonString);
            if (response != null && response.isSuccess && response.Data != null)
            {
                return Json(response.Data);
            }
            return Json(new List<GenreModel>());
        }

        [ActionName("Create")]
        public IActionResult GenreCreate()
        {
            return View("GenreCreate");
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> GenreSaveAsync(GenreCreateRequestModel requestModel)
        {
            _logger.LogInformation("Genre Save Async => Creating new genre");

            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                _logger.LogWarning("Genre Save Async => Genre name is required");
                return Json(new GenreCreateResponseModel { isSuccess = false, Message = "Genre name is required" });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            string json = JsonConvert.SerializeObject(requestModel);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
            var httpResponse = await client.PostAsync("api/genre", stringContent);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<GenreCreateResponseModel>(jsonString) ?? new GenreCreateResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Genre Save Async => Genre created successfully");
                TempData["Message"] = model.Message ?? "Genre created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Save Async => Failed to create genre: {model?.Message}");
            }

            return Json(model);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> GenreEditAsync(int id)
        {
            _logger.LogInformation($"Genre Edit Async => Fetching genre {id}");
            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.GetAsync($"api/genre/{id}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<GenreByIdResponseModel>(jsonString);
            
            if (model == null || !model.isSuccess || model.Data == null)
            {
                _logger.LogWarning($"Genre Edit Async => Failed to fetch genre {id}: {model?.Message}");
                TempData["isSuccess"] = false;
                TempData["Message"] = model?.Message ?? "Genre not found.";
                return Redirect("/Genre");
            }

            ViewData["Id"] = model.Data.GenreId;
            ViewData["GenreName"] = model.Data.GenreName;

            return View("GenreEdit", model.Data);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> GenreUpdateAsync(int id, GenrePatchRequestModel requestModel)
        {
            _logger.LogInformation($"Genre Update Async => Updating genre {id}");
            requestModel.GenreId = id;

            if (string.IsNullOrWhiteSpace(requestModel.GenreName))
            {
                return Json(new GenrePatchResponseModel { isSuccess = false, Message = "Genre name is required." });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            string json = JsonConvert.SerializeObject(requestModel);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, Application.Json);
            var httpResponse = await client.PatchAsync($"api/genre/{id}", stringContent);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<GenrePatchResponseModel>(jsonString) ?? new GenrePatchResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Genre Update Async => Genre updated successfully");
                TempData["Message"] = model.Message ?? "Genre updated successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Update Async => Failed to update genre: {model?.Message}");
            }

            return Json(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> GenreDeleteAsync(GenreDeleteRequestModel requestModel)
        {
            _logger.LogInformation($"Genre Delete Async => Deleting genre {requestModel.GenreId}");
            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.DeleteAsync($"api/genre/{requestModel.GenreId}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<GenreDeleteResponseModel>(jsonString) ?? new GenreDeleteResponseModel { isSuccess = false, Message = "Unknown error" };
            
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Genre Delete Async => Genre deleted successfully");
                TempData["Message"] = model.Message ?? "Genre deleted successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Delete Async => Failed to delete genre: {model?.Message}");
            }
            return Json(model);
        }
    }
}
