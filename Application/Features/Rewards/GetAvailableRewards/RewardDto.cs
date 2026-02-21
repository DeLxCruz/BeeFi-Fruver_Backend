using Domain.Enums;

namespace Application.Features.Rewards.GetAvailableRewards;

public record RewardDto(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    int PointsRequired,
    RewardType Type,
    decimal Value,
    bool IsExclusive,
    DateTime? ExpirationDate,
    int MaxRedemptionsPerUser,
    int UserRedemptionCount);
