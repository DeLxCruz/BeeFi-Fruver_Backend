using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.GetPendingUsers;

public class GetPendingUsersQueryHandler
    : IRequestHandler<GetPendingUsersQuery, Result<List<PendingUserDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PendingUserDto>>> Handle(
        GetPendingUsersQuery request,
        CancellationToken cancellationToken)
    {
        var pendingUsers = await _context.Users
            .Where(u => u.AccountStatus == AccountStatus.Pending)
            .OrderBy(u => u.CreatedAt)
            .Select(u => new PendingUserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.ProfileImageUrl,
                u.AccountStatus,
                u.CreatedAt,
                (int)(DateTime.UtcNow - u.CreatedAt).TotalDays))
            .ToListAsync(cancellationToken);

        return Result.Success(pendingUsers);
    }
}
