using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStorePOS.MvcApp.Models;

namespace BookStorePOS.MvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        try
        {
            return View();
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    public IActionResult Privacy()
    {
        try
        {
            return View();
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Privacy => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        try
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
}
