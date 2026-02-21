using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPushNotificationService _pushNotificationService;

    public SendNotificationCommandHandler(
        IApplicationDbContext context,
        IPushNotificationService pushNotificationService)
    {
        _context = context;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<Result> Handle(
        SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = Notification.Create(
            request.UserId,
            request.Type,
            request.Title,
            request.Message,
            request.Data);

        _context.Notifications.Add(notification);

        await _pushNotificationService.SendToUserAsync(
            request.UserId,
            request.Title,
            request.Message,
            request.Type,
            cancellationToken: cancellationToken);

        notification.MarkAsSent();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
