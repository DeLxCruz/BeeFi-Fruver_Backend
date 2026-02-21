using Application.Common.Models;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.GetMyRewards;

public record GetMyRewardsQuery(
    RewardStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<UserRewardDto>>>;
