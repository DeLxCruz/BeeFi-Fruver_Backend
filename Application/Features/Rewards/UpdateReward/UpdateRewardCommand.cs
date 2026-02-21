using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.UpdateReward;

public record UpdateRewardCommand(
    Guid RewardId,
    string Name,
    string Description,
    string? ImageUrl,
    int PointsRequired,
    decimal Value,
    bool IsActive,
    int MaxRedemptionsPerUser,
    DateTime? ExpirationDate) : IRequest<Result>;
