using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.SendNotification;

public record SendNotificationCommand(
    Guid UserId,
    string Title,
    string Message,
    NotificationType Type,
    string? Data = null) : IRequest<Result>;
