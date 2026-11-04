using System;

namespace BookStorePOS.Shared.Models.Notification;

public class NotificationModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "General"; // OrderPlaced, OrderConfirmed, OrderRejected, Announcement, LowStock
    public int? OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? TargetUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string? Metadata { get; set; }
}

public class AnnouncementRequestModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TargetAudience { get; set; } = "All"; // All, Staff, Customers
}

