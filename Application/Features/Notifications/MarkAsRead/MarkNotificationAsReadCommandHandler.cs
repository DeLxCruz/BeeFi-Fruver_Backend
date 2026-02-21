using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.MarkAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationAsReadCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification is null || notification.UserId != userId)
            return Result.Failure(new Error("Notification.NotFound", "Notificación no encontrada"));

        if (!notification.IsRead)
        {
            notification.MarkAsRead();
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
