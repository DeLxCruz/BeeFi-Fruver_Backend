namespace Application.Features.Rewards.RedeemReward;

public record RedeemRewardResponse(
    Guid UserRewardId,
    string RewardName,
    decimal RewardValue,
    DateTime ExpirationDate);
