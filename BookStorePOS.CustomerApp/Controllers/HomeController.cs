using Microsoft.AspNetCore.Mvc;

namespace BookStorePOS.CustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            var client = _httpClientFactory.CreateClient("WebAPI");
            var qs = $"?Page={page}&Limit=12&Title={Uri.EscapeDataString(search ?? "")}";
            var httpResponse = await client.GetAsync($"api/book{qs}");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<BookListResponseModel>(jsonString);
            
            return View(response ?? new BookListResponseModel());
        }
    }
}
