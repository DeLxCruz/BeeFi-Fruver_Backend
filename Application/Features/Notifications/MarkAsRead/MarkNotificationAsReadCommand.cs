using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.MarkAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<Result>;
