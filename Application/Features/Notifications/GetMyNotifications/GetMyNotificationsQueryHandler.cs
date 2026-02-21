using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.GetMyNotifications;

public class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, Result<PaginatedList<NotificationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (request.IsRead.HasValue)
            query = query.Where(n => n.IsRead == request.IsRead.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = notifications.Select(n => new NotificationDto(
            n.Id, n.Type, n.Title, n.Message,
            n.Data, n.IsRead, n.CreatedAt, n.ReadAt))
            .ToList();

        return Result.Success(new PaginatedList<NotificationDto>(
            dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
