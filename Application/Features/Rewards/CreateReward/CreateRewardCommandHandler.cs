using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.CreateReward;

public class CreateRewardCommandHandler : IRequestHandler<CreateRewardCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateRewardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateRewardCommand request,
        CancellationToken cancellationToken)
    {
        var reward = Reward.Create(
            name: request.Name,
            description: request.Description,
            pointsRequired: request.PointsRequired,
            type: request.Type,
            value: request.Value,
            isBeeFiExclusive: request.IsExclusive,
            maxRedemptionsPerUser: request.MaxRedemptionsPerUser,
            expirationDate: request.ExpirationDate,
            imageUrl: request.ImageUrl);

        _context.Rewards.Add(reward);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(reward.Id);
    }
}
