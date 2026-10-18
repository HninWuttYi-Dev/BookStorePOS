using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
        try
        {
                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={page}&Limit=12&Title={Uri.EscapeDataString(search ?? "")}";
                var httpResponse = await client.GetAsync($"api/book{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);
            
                return View(response ?? new BookListResponseModel());
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index => Exception occurred");
            return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

        [ActionName("NotFound")]
        public IActionResult PageNotFound()
        {
            try
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotFound => Exception occurred");
                return StatusCode(500, new { IsSuccess = false, Message = "Internal Server Error: " + ex.Message });
            }
        }
    }
}
