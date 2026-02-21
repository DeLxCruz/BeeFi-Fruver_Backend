using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.GetMyNotifications;

public record GetMyNotificationsQuery(
    bool? IsRead = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<NotificationDto>>>;
