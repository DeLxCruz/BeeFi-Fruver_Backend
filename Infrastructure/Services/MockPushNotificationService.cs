using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

// TODO: Reemplazar con Firebase Cloud Messaging (FCM)
public class MockPushNotificationService : IPushNotificationService
{
    private readonly ILogger<MockPushNotificationService> _logger;

    public MockPushNotificationService(ILogger<MockPushNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendToUserAsync(Guid userId, string title, string body,
        NotificationType type, object? data = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK PUSH] → Usuario {UserId} | Tipo: {Type} | Título: {Title} | Cuerpo: {Body}",
            userId, type, title, body);
        return Task.CompletedTask;
    }

    public Task SendToMultipleUsersAsync(IEnumerable<Guid> userIds,
        string title, string body, NotificationType type,
        object? data = null,
        CancellationToken cancellationToken = default)
    {
        var ids = string.Join(", ", userIds);
        _logger.LogInformation(
            "[MOCK PUSH] → Usuarios [{UserIds}] | Tipo: {Type} | Título: {Title} | Cuerpo: {Body}",
            ids, type, title, body);
        return Task.CompletedTask;
    }

    public Task SendToTopicAsync(string topic, string title, string body,
        NotificationType type, object? data = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK PUSH] → Tópico: {Topic} | Tipo: {Type} | Título: {Title} | Cuerpo: {Body}",
            topic, type, title, body);
        return Task.CompletedTask;
    }
}
