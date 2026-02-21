using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task SendToUserAsync(Guid userId, string title, string body,
        NotificationType type, object? data = null,
        CancellationToken cancellationToken = default);

    Task SendToMultipleUsersAsync(IEnumerable<Guid> userIds,
        string title, string body, NotificationType type,
        object? data = null,
        CancellationToken cancellationToken = default);

    Task SendToTopicAsync(string topic, string title, string body,
        NotificationType type, object? data = null,
        CancellationToken cancellationToken = default);
}
