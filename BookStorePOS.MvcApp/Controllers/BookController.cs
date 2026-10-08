using Microsoft.AspNetCore.Mvc;
namespace BookStorePOS.MvcApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookController> _logger;

        public BookController(IHttpClientFactory httpClientFactory, ILogger<BookController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> BookListAsync(BookListRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation("Book List Async => Fetching books");

                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={requestModel.Page}&Limit={requestModel.Limit}&SearchQuery={Uri.EscapeDataString(requestModel.SearchQuery ?? "")}&Isbn={Uri.EscapeDataString(requestModel.Isbn ?? "")}&Title={Uri.EscapeDataString(requestModel.Title ?? "")}&Author={Uri.EscapeDataString(requestModel.Author ?? "")}&Genre={Uri.EscapeDataString(requestModel.Genre ?? "")}";
                var httpResponse = await client.GetAsync($"api/book{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);

                if (response != null && response.isSuccess && response.Data != null)
                {
                    ViewData["Books"] = response.Data;
                    ViewData["CurrentPage"] = response.Page;
                    ViewData["TotalPages"] = response.TotalPages;
                    ViewData["IsbnFilter"] = requestModel.Isbn;
                    ViewData["TitleFilter"] = requestModel.Title;
                    ViewData["AuthorFilter"] = requestModel.Author;
                    ViewData["GenreFilter"] = requestModel.Genre;
                    _logger.LogInformation("Book List Async => Books fetched successfully");
                }
                else
                {
                    _logger.LogWarning($"Book List Async => Failed to fetch books: {response?.Message}");
                    TempData["Message"] = response?.Message;
                    TempData["isSuccess"] = false;
                }

                return View("BookList");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookListAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("Detail")]
        public async Task<IActionResult> BookDetailAsync(int id, int? editionId = null)
        {
        try
        {
                _logger.LogInformation($"Book Detail Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (response == null || !response.isSuccess || response.Data == null)
                {
                    _logger.LogWarning($"Book Detail Async => Failed to fetch book {id}");
                    TempData["Message"] = "Book not found.";
                    TempData["isSuccess"] = false;
                    return RedirectToAction("Index");
                }

                if (editionId.HasValue && response.Data.Editions != null)
                {
                    var selectedEdition = response.Data.Editions.FirstOrDefault(e => e.BookEditionId == editionId.Value);
                    if (selectedEdition != null)
                    {
                        ViewData["SelectedEdition"] = selectedEdition;
                    }
                }

                ViewData["Book"] = response.Data;
                return View("BookDetail");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookDetailAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("Create")]
        public IActionResult BookCreate()
        {
        try
        {
                return View("BookCreate");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookCreate => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> BookSaveAsync(BookCreateRequestModel requestModel, IFormFile? photo)
        {
        try
        {
                _logger.LogInformation("Book Save Async => Creating new book");



                if (string.IsNullOrWhiteSpace(requestModel.Title))
                {
                    _logger.LogWarning("Book Save Async => Title is required");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "Title is required" });
                }
                if (string.IsNullOrWhiteSpace(requestModel.Author))
                {
                    _logger.LogWarning("Book Save Async => Author is required");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "Author is required" });
                }
                if (string.IsNullOrWhiteSpace(requestModel.Genre))
                {
                    _logger.LogWarning("Book Save Async => Genre is required");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "Genre is required" });
                }
                if (requestModel.Price <= 0)
                {
                    _logger.LogWarning("Book Save Async => Price must be greater than zero");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "Price must be greater than zero" });
                }
                if (requestModel.StockQuantity < 0)
                {
                    _logger.LogWarning("Book Save Async => StockQuantity must be greater or zero");
                    return Json(new BookCreateResponseModel { isSuccess = false, Message = "StockQuantity must be greater or equal to zero" });
                }

                if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
                {
                    var isbn = requestModel.Isbn.Replace("-", "").Replace(" ", "");
                    if (isbn.Length != 10 && isbn.Length != 13)
                    {
                        _logger.LogWarning("Book Save Async => ISBN must be 10 or 13 digits");
                        return Json(new BookCreateResponseModel { isSuccess = false, Message = "ISBN must be 10 or 13 digits." });
                    }
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                using var content = new MultipartFormDataContent();

                if (requestModel.Title != null) content.Add(new StringContent(requestModel.Title), "Title");
                if (requestModel.Author != null) content.Add(new StringContent(requestModel.Author), "Author");
                if (requestModel.Genre != null) content.Add(new StringContent(requestModel.Genre), "Genre");
                if (requestModel.Isbn != null) content.Add(new StringContent(requestModel.Isbn), "Isbn");
                content.Add(new StringContent(requestModel.Price.ToString()), "Price");
                content.Add(new StringContent(requestModel.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(requestModel.ReorderLevel.ToString()), "ReorderLevel");
                if (requestModel.EditionName != null) content.Add(new StringContent(requestModel.EditionName), "EditionName");
                if (requestModel.Description != null) content.Add(new StringContent(requestModel.Description), "Description");
                if (requestModel.EditionNote != null) content.Add(new StringContent(requestModel.EditionNote), "EditionNote");
                if (requestModel.PublishDate.HasValue) content.Add(new StringContent(requestModel.PublishDate.Value.ToString("O")), "PublishDate");
                if (requestModel.PageCount.HasValue) content.Add(new StringContent(requestModel.PageCount.Value.ToString()), "PageCount");

                if (photo != null && photo.Length > 0)
                {
                    var streamContent = new StreamContent(photo.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                    content.Add(streamContent, "photo", photo.FileName);
                }

                var httpResponse = await client.PostAsync("api/book", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookCreateResponseModel>(jsonString) ?? new BookCreateResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Book Save Async => Book created successfully");
                    TempData["Message"] = model.Message ?? "Book created successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Book Save Async => Failed to create book: {model?.Message}");
                }

                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookSaveAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
        [ActionName("Edit")]
        public async Task<IActionResult> BookEditAsync(int id)
        {
        try
        {
                _logger.LogInformation($"Book Edit Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (model == null || !model.isSuccess)
                {
                    _logger.LogWarning($"Book Edit Async => Failed to fetch book {id}: {model?.Message}");
                    TempData["isSuccess"] = false;
                    TempData["Message"] = model?.Message;
                    return Redirect("/Book");
                }

                ViewData["Id"] = model.Data.BookId;
                ViewData["BookTitle"] = model.Data.Title;
                ViewData["Author"] = model.Data.Author;
                ViewData["Genre"] = model.Data.Genre;
                ViewData["AuthorId"] = model.Data.AuthorId;
                ViewData["GenreId"] = model.Data.GenreId;
                ViewData["Description"] = model.Data.Description;
                
                var firstEdition = model.Data.Editions?.FirstOrDefault();
                if (firstEdition != null)
                {
                    ViewData["Isbn"] = firstEdition.Isbn;
                    ViewData["Price"] = firstEdition.Price;
                    ViewData["StockQuantity"] = firstEdition.StockQuantity;
                    ViewData["ReorderLevel"] = firstEdition.ReorderLevel;
                    ViewData["CoverImageUrl"] = firstEdition.CoverImageUrl;
                }

                return View("BookEdit", model.Data);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookEditAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> BookUpdateAsync(int id, BookPatchRequestModel requestModel, IFormFile? photo)
        {
        try
        {
                _logger.LogInformation($"Book Update Async => Updating book {id}");
                requestModel.BookId = id;
                if (string.IsNullOrWhiteSpace(requestModel.Title) 
                    && string.IsNullOrWhiteSpace(requestModel.Author) 
                    && string.IsNullOrWhiteSpace(requestModel.Genre)
                    && string.IsNullOrWhiteSpace(requestModel.Description)
                    && photo == null)
                {
                    return Json(new BookPatchResponseModel { isSuccess = false, Message = "Please update at least one field." });
                }

                var client = _httpClientFactory.CreateClient("WebAPI");
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(id.ToString()), "BookId");
                if (requestModel.Title != null) content.Add(new StringContent(requestModel.Title), "Title");
                if (requestModel.Author != null) content.Add(new StringContent(requestModel.Author), "Author");
                if (requestModel.Genre != null) content.Add(new StringContent(requestModel.Genre), "Genre");
                if (requestModel.Description != null) content.Add(new StringContent(requestModel.Description), "Description");
                
                if (requestModel.Price.HasValue) content.Add(new StringContent(requestModel.Price.Value.ToString()), "Price");
                if (requestModel.StockQuantity.HasValue) content.Add(new StringContent(requestModel.StockQuantity.Value.ToString()), "StockQuantity");
                if (requestModel.ReorderLevel.HasValue) content.Add(new StringContent(requestModel.ReorderLevel.Value.ToString()), "ReorderLevel");
                if (requestModel.Isbn != null) content.Add(new StringContent(requestModel.Isbn), "Isbn");

                if (photo != null && photo.Length > 0)
                {
                    var streamContent = new StreamContent(photo.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                    content.Add(streamContent, "photo", photo.FileName);
                }

                var httpResponse = await client.PatchAsync($"api/book/{id}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookPatchResponseModel>(jsonString) ?? new BookPatchResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Book Update Async => Book updated successfully");
                    TempData["Message"] = model.Message ?? "Book updated successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Book Update Async => Failed to update book: {model?.Message}");
                }

                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookUpdateAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> BookDeleteAsync(BookDeleteRequestModel requestModel)
        {
        try
        {
                _logger.LogInformation($"Book Delete Async => Deleting book {requestModel.BookId}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.DeleteAsync($"api/book/{requestModel.BookId}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookDeleteResponseModel>(jsonString) ?? new BookDeleteResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Book Delete Async => Book deleted successfully");
                    TempData["Message"] = model.Message ?? "Book deleted successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Book Delete Async => Failed to delete book: {model?.Message}");
                }
                return Json(model);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookDeleteAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
        try
        {
                return View("Error!");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPost]
    [ActionName("EditionSave")]
    public async Task<IActionResult> EditionSaveAsync(int id, BookEditionCreateRequestModel requestModel, IFormFile? photo)
    {
        try
        {
            _logger.LogInformation($"Book Create Edition Async => Creating edition for book {id}");
            requestModel.BookId = id;
            if (string.IsNullOrWhiteSpace(requestModel.EditionName))
            {
                return Json(new { isSuccess = false, Message = "Edition Name is required." });
            }
            if (requestModel.Price <= 0)
            {
                return Json(new { isSuccess = false, Message = "Price must be greater than zero." });
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(id.ToString()), "BookId");
            content.Add(new StringContent(requestModel.EditionName), "EditionName");
            if (requestModel.Isbn != null) content.Add(new StringContent(requestModel.Isbn), "Isbn");
            content.Add(new StringContent(requestModel.Price.ToString()), "Price");
            content.Add(new StringContent(requestModel.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(requestModel.ReorderLevel.ToString()), "ReorderLevel");
            if (requestModel.EditionNote != null) content.Add(new StringContent(requestModel.EditionNote), "EditionNote");
            if (requestModel.PublishDate.HasValue) content.Add(new StringContent(requestModel.PublishDate.Value.ToString("O")), "PublishDate");
            if (requestModel.PageCount.HasValue) content.Add(new StringContent(requestModel.PageCount.Value.ToString()), "PageCount");

            if (photo != null && photo.Length > 0)
            {
                var streamContent = new StreamContent(photo.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                content.Add(streamContent, "photo", photo.FileName);
            }

            var httpResponse = await client.PostAsync($"api/book/{id}/edition", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<BookEditionCreateResponseModel>(jsonString) ?? new BookEditionCreateResponseModel { isSuccess = false, Message = "Unknown error" };
        
            if (model != null && model.isSuccess)
            {
                _logger.LogInformation("Book Create Edition Async => Edition created successfully");
                TempData["Message"] = model.Message ?? "Edition created successfully.";
                TempData["isSuccess"] = true;
            }
            else
            {
                _logger.LogWarning($"Book Create Edition Async => Failed to create edition: {model?.Message}");
            }

            return Json(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateEditionAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
        [ActionName("CreateEdition")]
        public async Task<IActionResult> CreateEditionAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Book Create Edition Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (model == null || !model.isSuccess)
                {
                    _logger.LogWarning($"Book Create Edition Async => Failed to fetch book {id}: {model?.Message}");
                    TempData["isSuccess"] = false;
                    TempData["Message"] = model?.Message;
                    return Redirect("/Book");
                }

                ViewData["Id"] = model.Data.BookId;
                ViewData["BookTitle"] = model.Data.Title;
                ViewData["Author"] = model.Data.Author;
                ViewData["Genre"] = model.Data.Genre;
                ViewData["Description"] = model.Data.Description;
                
                return View("BookCreateEdition", model.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookCreateEditionAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
        [ActionName("EditEdition")]
        public async Task<IActionResult> EditEditionAsync(int id, int editionId)
        {
            try
            {
                _logger.LogInformation($"Book Edit Edition Async => Fetching book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var httpResponse = await client.GetAsync($"api/book/{id}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookByIdResponseModel>(jsonString);
            
                if (model == null || !model.isSuccess)
                {
                    _logger.LogWarning($"Book Edit Edition Async => Failed to fetch book {id}: {model?.Message}");
                    TempData["isSuccess"] = false;
                    TempData["Message"] = model?.Message;
                    return Redirect("/Book");
                }

                var edition = model.Data?.Editions?.FirstOrDefault(e => e.BookEditionId == editionId);
                if (edition == null)
                {
                    TempData["isSuccess"] = false;
                    TempData["Message"] = "Edition not found.";
                    return Redirect("/Book");
                }

                ViewData["Id"] = model.Data!.BookId;
                ViewData["BookTitle"] = model.Data.Title;
                ViewData["Author"] = model.Data.Author;
                ViewData["Genre"] = model.Data.Genre;
                ViewData["Description"] = model.Data.Description;
                
                ViewData["EditionId"] = edition.BookEditionId;
                ViewData["EditionName"] = edition.EditionName;
                ViewData["Isbn"] = edition.Isbn;
                ViewData["Price"] = edition.Price;
                ViewData["StockQuantity"] = edition.StockQuantity;
                ViewData["ReorderLevel"] = edition.ReorderLevel;
                ViewData["CoverImageUrl"] = edition.CoverImageUrl;
                ViewData["PublishDate"] = edition.PublishDate;
                ViewData["PageCount"] = edition.PageCount;
                ViewData["EditionNote"] = edition.EditionNote;
                
                return View("BookEditEdition", model.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookEditEditionAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ActionName("UpdateEditionSave")]
        public async Task<IActionResult> UpdateEditionSaveAsync(int id, int editionId, BookEditionPatchRequestModel requestModel, IFormFile? photo)
        {
            try
            {
                _logger.LogInformation($"Book Update Edition Async => Updating edition {editionId} for book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(requestModel.EditionName ?? ""), "EditionName");
                content.Add(new StringContent(requestModel.Isbn ?? ""), "Isbn");
                content.Add(new StringContent(requestModel.Price.ToString()), "Price");
                content.Add(new StringContent(requestModel.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(requestModel.ReorderLevel.ToString()), "ReorderLevel");
                if (requestModel.EditionNote != null) content.Add(new StringContent(requestModel.EditionNote), "EditionNote");
                if (requestModel.PublishDate.HasValue) content.Add(new StringContent(requestModel.PublishDate.Value.ToString("O")), "PublishDate");
                if (requestModel.PageCount.HasValue) content.Add(new StringContent(requestModel.PageCount.Value.ToString()), "PageCount");

                if (photo != null && photo.Length > 0)
                {
                    var streamContent = new StreamContent(photo.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(photo.ContentType);
                    content.Add(streamContent, "photo", photo.FileName);
                }

                var httpResponse = await client.PatchAsync($"api/book/{id}/edition/{editionId}", content);
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookEditionPatchResponseModel>(jsonString) ?? new BookEditionPatchResponseModel { isSuccess = false, Message = "Unknown error" };

                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Book Update Edition Async => Edition updated successfully");
                    TempData["Message"] = model.Message ?? "Edition updated successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Book Update Edition Async => Failed to update edition: {model?.Message}");
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateEditionSaveAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex?.InnerException?.Message });
            
            }
        }
        [HttpPost]
        [ActionName("DeleteEdition")]
        public async Task<IActionResult> DeleteEditionAsync(int id, int editionId)
        {
            try
            {
                _logger.LogInformation($"Book Delete Edition Async => Deleting edition {editionId} for book {id}");
                var client = _httpClientFactory.CreateClient("WebAPI");

                var httpResponse = await client.DeleteAsync($"api/book/{id}/edition/{editionId}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<BookEditionDeleteResponseModel>(jsonString) ?? new BookEditionDeleteResponseModel { isSuccess = false, Message = "Unknown error" };
            
                if (model != null && model.isSuccess)
                {
                    _logger.LogInformation("Book Delete Edition Async => Edition deleted successfully");
                    TempData["Message"] = model.Message ?? "Edition deleted successfully.";
                    TempData["isSuccess"] = true;
                }
                else
                {
                    _logger.LogWarning($"Book Delete Edition Async => Failed to delete edition: {model?.Message}");
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteEditionAsync => Exception occurred");
                return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
