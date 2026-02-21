using Domain.Enums;

namespace API.Contracts.Rewards;

public record CreateRewardRequest(
    string Name,
    string Description,
    string? ImageUrl,
    int PointsRequired,
    RewardType Type,
    decimal Value,
    bool IsExclusive,
    int MaxRedemptionsPerUser,
    DateTime? ExpirationDate);
