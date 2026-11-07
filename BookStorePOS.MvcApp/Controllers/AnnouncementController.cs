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
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Announcement" });
        }

        var userRole = Request.Cookies["pos_role"] ?? "";
        var userPerms = Request.Cookies["pos_permissions"] ?? "";
        bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

        // Dynamic RBAC: Broadcast Alerts is an Admin feature under SETTINGS.
        // If not Admin, dynamically route user to the first module they have permission for.
        if (!isAdmin)
        {
            var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (permList.Contains("Dashboard.View", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (permList.Contains("Order.Create", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Checkout");
            }
            if (permList.Contains("Book.View", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Book");
            }
            if (permList.Contains("Order.View", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Orders");
            }

            return RedirectToAction("Login", "Auth");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(AnnouncementRequestModel requestModel)
    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        var hasToken = !string.IsNullOrEmpty(Request.Cookies["pos_token"]);
        var hasRefresh = !string.IsNullOrEmpty(Request.Cookies["pos_refresh"]);
        if (!hasToken && !hasRefresh)
        {
            if (isAjax) return StatusCode(401, new { success = false, message = "Session expired. Please log in again." });
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Announcement" });
        }

        var userRole = Request.Cookies["pos_role"] ?? "";
        var userPerms = Request.Cookies["pos_permissions"] ?? "";
        bool isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

        // Dynamic RBAC: Only Admin can send store-wide broadcast announcements
        if (!isAdmin)
        {
            if (isAjax) return StatusCode(403, new { success = false, message = "Access Denied: Only administrators can broadcast announcements." });

            var permList = userPerms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (permList.Contains("Order.Create", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Checkout");
            }
            if (permList.Contains("Dashboard.View", StringComparer.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return RedirectToAction("Login", "Auth");
        }

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

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || HttpContext.Items.ContainsKey("Auth_Expired"))
            {
                if (isAjax) return StatusCode(401, new { success = false, message = "Session expired. Please log in again." });
                return RedirectToAction("Login", "Auth", new { returnUrl = "/Announcement" });
            }

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

