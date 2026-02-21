using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.RedeemReward;

public record RedeemRewardCommand(Guid RewardId) : IRequest<Result<RedeemRewardResponse>>;
