using Domain.Enums;

namespace API.Contracts.Notifications;

public record SendNotificationRequest(
    Guid UserId,
    string Title,
    string Message,
    NotificationType Type,
    string? Data = null);
