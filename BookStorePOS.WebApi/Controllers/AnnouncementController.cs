using System;
using System.Threading.Tasks;
using BookStorePOS.Domain.Features.Auth;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.Notification;
using BookStorePOS.WebApi.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementController : BaseController
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(
        IHubContext<NotificationHub> hubContext,
        ILogger<AnnouncementController> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SendAnnouncement([FromBody] AnnouncementRequestModel requestModel)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestModel.Title) || string.IsNullOrWhiteSpace(requestModel.Content))
            {
                return ExecuteResult(Result<string>.ValidationError("Title and Content are required."));
            }

            _logger.LogInformation("Broadcasting announcement: {Title}", requestModel.Title);

            var notification = new NotificationModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = requestModel.Title,
                Message = requestModel.Content,
                Type = "Announcement",
                CreatedAt = DateTime.UtcNow
            };

            if (string.Equals(requestModel.TargetAudience, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                await _hubContext.Clients.Group("Staff").SendAsync("ReceiveNotification", notification);
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            }

            return ExecuteResult(Result<string>.SuccessResult("Announcement broadcasted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast announcement");
            return ExecuteResult(Result<string>.SystemError("Internal error: " + ex.Message));
        }
    }
}
