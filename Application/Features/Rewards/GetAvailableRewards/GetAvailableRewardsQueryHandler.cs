using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rewards.GetAvailableRewards;

public class GetAvailableRewardsQueryHandler
    : IRequestHandler<GetAvailableRewardsQuery, Result<PaginatedList<RewardDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAvailableRewardsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<RewardDto>>> Handle(
        GetAvailableRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUser.UserId;

        var query = _context.Rewards
            .AsNoTracking()
            .Where(r => r.IsActive && (r.ExpirationDate == null || r.ExpirationDate > now));

        if (request.Type.HasValue)
            query = query.Where(r => r.Type == request.Type.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var rewards = await query
            .OrderBy(r => r.PointsRequired)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // UserRedemptionCount subquery
        var rewardIds = rewards.Select(r => r.Id).ToList();
        Dictionary<Guid, int> redemptionCounts = userId.HasValue
            ? await _context.UserRewards
                .Where(ur => ur.UserId == userId.Value && rewardIds.Contains(ur.RewardId))
                .GroupBy(ur => ur.RewardId)
                .Select(g => new { RewardId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RewardId, x => x.Count, cancellationToken)
            : new Dictionary<Guid, int>();

        var dtos = rewards.Select(r => new RewardDto(
            r.Id, r.Name, r.Description, r.ImageUrl,
            r.PointsRequired, r.Type, r.Value,
            r.IsBeeFiExclusive, r.ExpirationDate,
            r.MaxRedemptionsPerUser,
            redemptionCounts.GetValueOrDefault(r.Id, 0)))
            .ToList();

        return Result.Success(new PaginatedList<RewardDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
