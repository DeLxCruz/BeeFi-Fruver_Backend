using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.MarkAllAsRead;

public record MarkAllNotificationsAsReadCommand : IRequest<Result>;
