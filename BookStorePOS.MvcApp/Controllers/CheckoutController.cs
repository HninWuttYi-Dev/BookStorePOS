using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Book;

namespace BookStorePOS.MvcApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IHttpClientFactory httpClientFactory, ILogger<CheckoutController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> CheckoutAsync(int page = 1, string search = "")
        {
        try
        {
                _logger.LogInformation("Checkout Index => Fetching books");
                var client = _httpClientFactory.CreateClient("WebAPI");
                var qs = $"?Page={page}&Limit=12&Title={Uri.EscapeDataString(search ?? "")}";
                var httpResponse = await client.GetAsync($"api/book{qs}");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);
            
                if (response != null && response.isSuccess && response.Data != null)
                {
                    ViewData["Books"] = response.Data;
                    ViewData["CurrentPage"] = response.Page;
                    ViewData["TotalPages"] = response.TotalPages;
                    ViewData["Search"] = search;
                }
            
                return View("Checkout");
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckoutAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
    }
}
