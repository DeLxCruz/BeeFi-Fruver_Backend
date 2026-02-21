using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.RegisterDeviceToken;

public class RegisterDeviceTokenCommandHandler : IRequestHandler<RegisterDeviceTokenCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RegisterDeviceTokenCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RegisterDeviceTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var existing = await _context.DeviceTokens
            .FirstOrDefaultAsync(dt => dt.UserId == userId && dt.Token == request.Token, cancellationToken);

        if (existing is not null)
        {
            existing.Reactivate();
        }
        else
        {
            var token = DeviceToken.Create(userId, request.Token, request.Platform);
            _context.DeviceTokens.Add(token);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
