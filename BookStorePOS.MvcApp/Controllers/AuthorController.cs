using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Domain.Features.Author;
using BookStorePOS.Domain.Models.Author;

namespace BookStorePOS.MvcApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> AuthorListAsync(AuthorListRequestModel requestModel)
        {
            _logger.LogInformation("Author List Async => Fetching authors");

            var response = await _authorService.GetAuthorsAsync(requestModel);

            if (response.isSuccess && response.Data != null)
            {
                ViewData["Authors"] = response.Data;
                ViewData["CurrentPage"] = response.Page;
                ViewData["TotalPages"] = response.TotalPages;
                ViewData["AuthorNameFilter"] = requestModel.AuthorName;
                _logger.LogInformation("Author List Async => Authors fetched successfully");
            }
            else
            {
                _logger.LogWarning($"Author List Async => Failed to fetch authors: {response.Message}");
                TempData["Message"] = response.Message;
                TempData["isSuccess"] = false;
            }

            return View("AuthorList");
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> AuthorSearchAsync(string query)
        {
            var request = new AuthorListRequestModel { AuthorName = query, Page = 1, Limit = 10 };
            var response = await _authorService.GetAuthorsAsync(request);
            if (response.isSuccess && response.Data != null)
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

            var model = await _authorService.CreateAuthorAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Author Save Async => Author created successfully");
                TempData["Message"] = model.Message ?? "Author created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Save Async => Failed to create author: {model.Message}");
            }

            return Json(model);
        }

        [ActionName("Edit")]
        public async Task<IActionResult> AuthorEditAsync(int id)
        {
            _logger.LogInformation($"Author Edit Async => Fetching author {id}");
            var model = await _authorService.GetAuthorByIdAsync(new AuthorByIdRequestModel { AuthorId = id });
            
            if (!model.isSuccess || model.Data == null)
            {
                _logger.LogWarning($"Author Edit Async => Failed to fetch author {id}: {model.Message}");
                TempData["isSuccess"] = false;
                TempData["Message"] = model.Message ?? "Author not found.";
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

            var model = await _authorService.UpdateAuthorAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Author Update Async => Author updated successfully");
                TempData["Message"] = model.Message ?? "Author updated successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Update Async => Failed to update author: {model.Message}");
            }

            return Json(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> AuthorDeleteAsync(AuthorDeleteRequestModel requestModel)
        {
            _logger.LogInformation($"Author Delete Async => Deleting author {requestModel.AuthorId}");
            var model = await _authorService.DeleteAuthorAsync(requestModel);
            
            if (model.isSuccess)
            {
                _logger.LogInformation("Author Delete Async => Author deleted successfully");
                TempData["Message"] = model.Message ?? "Author deleted successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Author Delete Async => Failed to delete author: {model.Message}");
            }
            return Json(model);
        }
    }
}
