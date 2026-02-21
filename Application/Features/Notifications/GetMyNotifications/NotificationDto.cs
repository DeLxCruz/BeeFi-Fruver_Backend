using Domain.Enums;

namespace Application.Features.Notifications.GetMyNotifications;

public record NotificationDto(
    Guid Id,
    NotificationType Type,
    string Title,
    string Message,
    string? Data,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt);
