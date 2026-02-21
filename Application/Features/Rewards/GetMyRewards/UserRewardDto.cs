using Domain.Enums;

namespace Application.Features.Rewards.GetMyRewards;

public record UserRewardDto(
    Guid Id,
    Guid RewardId,
    string RewardName,
    RewardType RewardType,
    decimal RewardValue,
    RewardStatus Status,
    DateTime RedeemedAt,
    DateTime? ExpirationDate,
    DateTime? UsedAt);
