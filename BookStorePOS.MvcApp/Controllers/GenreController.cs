using Microsoft.AspNetCore.Mvc;

namespace BookStorePOS.MvcApp.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenreService genreService, ILogger<GenreController> logger)
        {
            _genreService = genreService;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> GenreListAsync(GenreListRequestModel requestModel)
        {
            _logger.LogInformation("Genre List Async => Fetching genres");

            var response = await _genreService.GetGenresAsync(requestModel);

            if (response.isSuccess && response.Data != null)
            {
                ViewData["Genres"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["GenreNameFilter"] = requestModel.GenreName;
                _logger.LogInformation("Genre List Async => Genres fetched successfully");
            }
            else
            {
                _logger.LogWarning($"Genre List Async => Failed to fetch genres: {response.Message}");
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
            }

            return View("GenreList");
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

            var model = await _genreService.CreateGenreAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Genre Save Async => Genre created successfully");
                TempData["Message"] = model.Message ?? "Genre created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Save Async => Failed to create genre: {model.Message}");
            }

            return Json(model);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> GenreEditAsync(int id)
        {
            _logger.LogInformation($"Genre Edit Async => Fetching genre {id}");
            var model = await _genreService.GetGenreByIdAsync(new GenreByIdRequestModel { GenreId = id });
            
            if (!model.isSuccess || model.Data == null)
            {
                _logger.LogWarning($"Genre Edit Async => Failed to fetch genre {id}: {model.Message}");
                TempData["isSuccess"] = false;
                TempData["Message"] = model.Message ?? "Genre not found.";
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

            var model = await _genreService.UpdateGenreAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Genre Update Async => Genre updated successfully");
                TempData["Message"] = model.Message ?? "Genre updated successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Update Async => Failed to update genre: {model.Message}");
            }

            return Json(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> GenreDeleteAsync(GenreDeleteRequestModel requestModel)
        {
            _logger.LogInformation($"Genre Delete Async => Deleting genre {requestModel.GenreId}");
            var model = await _genreService.DeleteGenreAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Genre Delete Async => Genre deleted successfully");
                TempData["Message"] = model.Message ?? "Genre deleted successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Genre Delete Async => Failed to delete genre: {model.Message}");
            }
            return Json(model);
        }
    }
}
