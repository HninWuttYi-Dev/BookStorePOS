using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.MvcApp.Controllers;

public class AnnouncementController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(IHttpClientFactory httpClientFactory, ILogger<AnnouncementController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
        var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
        if (!hasToken && !hasRefresh)
        {
            return RedirectToAction("Login", "Auth");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(AnnouncementRequestModel requestModel)
    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.Title) || string.IsNullOrWhiteSpace(requestModel.Content))
            {
                if (isAjax) return Json(new { success = false, message = "Title and Announcement message cannot be empty." });
                TempData["ErrorMessage"] = "Title and Announcement message cannot be empty.";
                TempData["IsSuccess"] = false;
                return View("Index", requestModel);
            }

            var client = _httpClientFactory.CreateClient("WebAPI");
            var response = await client.PostAsJsonAsync("api/announcement", requestModel);

            if (response.IsSuccessStatusCode)
            {
                if (isAjax) return Json(new { success = true, message = $"Announcement '{requestModel.Title}' sent successfully!" });
                TempData["SuccessMessage"] = $"Announcement '{requestModel.Title}' sent successfully!";
                TempData["IsSuccess"] = true;
                return RedirectToAction("Index");
            }

            var errResult = await response.Content.ReadFromJsonAsync<Result<string>>();
            var msg = errResult?.Message ?? "Failed to broadcast announcement.";
            if (isAjax) return Json(new { success = false, message = msg });
            TempData["ErrorMessage"] = msg;
            TempData["IsSuccess"] = false;
            return View("Index", requestModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while broadcasting announcement");
            var msg = "Unexpected error: " + ex.Message;
            if (isAjax) return Json(new { success = false, message = msg });
            TempData["ErrorMessage"] = msg;
            TempData["IsSuccess"] = false;
            return View("Index", requestModel);
        }
    }
}

