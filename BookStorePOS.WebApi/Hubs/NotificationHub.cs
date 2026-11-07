using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using BookStorePOS.Shared.Models.Notification;

namespace BookStorePOS.WebApi.Hubs;

public class NotificationHub : Hub
{
    /// <summary>
    /// Staff members join the Staff group to receive online order alerts and store events
    /// </summary>
    public async Task JoinStaffGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Staff");
    }

    /// <summary>
    /// Staff members can leave the Staff group
    /// </summary>
    public async Task LeaveStaffGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Staff");
    }

    /// <summary>
    /// Customer clients join the Customers group to receive store-wide customer announcements
    /// </summary>
    public async Task JoinCustomerGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Customers");
    }

    /// <summary>
    /// Customer clients can leave the Customers group
    /// </summary>
    public async Task LeaveCustomerGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Customers");
    }

    /// <summary>
    /// Customer clients join their personal user group to receive personal order status updates
    /// </summary>
    public async Task JoinUserGroup(string userId)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
    }

    /// <summary>
    /// Broadcast an announcement to all connected clients
    /// </summary>
    public async Task SendAnnouncement(string title, string content)
    {
        var model = new NotificationModel
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title,
            Message = content,
            Type = "Announcement",
            TargetUrl = null,
            CreatedAt = DateTime.UtcNow
        };

        // Send unified NotificationModel to all clients
        await Clients.All.SendAsync("ReceiveNotification", model);
    }
}

