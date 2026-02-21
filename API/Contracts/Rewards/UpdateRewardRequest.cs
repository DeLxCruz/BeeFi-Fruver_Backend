namespace API.Contracts.Rewards;

public record UpdateRewardRequest(
    string Name,
    string Description,
    string? ImageUrl,
    int PointsRequired,
    decimal Value,
    bool IsActive,
    int MaxRedemptionsPerUser,
    DateTime? ExpirationDate);
