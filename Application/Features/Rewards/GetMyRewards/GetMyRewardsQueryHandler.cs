using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rewards.GetMyRewards;

public class GetMyRewardsQueryHandler
    : IRequestHandler<GetMyRewardsQuery, Result<PaginatedList<UserRewardDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyRewardsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<UserRewardDto>>> Handle(
        GetMyRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.UserRewards
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Reward);

        var filtered = request.Status.HasValue
            ? query.Where(ur => ur.Status == request.Status.Value)
            : query;

        var totalCount = await filtered.CountAsync(cancellationToken);

        var rewards = await filtered
            .OrderByDescending(ur => ur.RedeemedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = rewards.Select(ur => new UserRewardDto(
            ur.Id, ur.RewardId, ur.Reward.Name,
            ur.Reward.Type, ur.Reward.Value,
            ur.Status, ur.RedeemedAt,
            ur.ExpirationDate, ur.UsedAt))
            .ToList();

        return Result.Success(new PaginatedList<UserRewardDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
