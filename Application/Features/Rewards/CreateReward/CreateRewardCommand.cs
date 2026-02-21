using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.CreateReward;

public record CreateRewardCommand(
    string Name,
    string Description,
    string? ImageUrl,
    int PointsRequired,
    RewardType Type,
    decimal Value,
    bool IsExclusive,
    int MaxRedemptionsPerUser,
    DateTime? ExpirationDate) : IRequest<Result<Guid>>;
