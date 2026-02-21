using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rewards.UpdateReward;

public class UpdateRewardCommandHandler : IRequestHandler<UpdateRewardCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateRewardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateRewardCommand request,
        CancellationToken cancellationToken)
    {
        var reward = await _context.Rewards
            .FirstOrDefaultAsync(r => r.Id == request.RewardId, cancellationToken);

        if (reward is null)
            return Result.Failure(RewardErrors.NotFound);

        reward.Update(
            request.Name,
            request.Description,
            request.ImageUrl,
            request.PointsRequired,
            request.Value,
            request.IsActive,
            request.MaxRedemptionsPerUser,
            request.ExpirationDate);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
