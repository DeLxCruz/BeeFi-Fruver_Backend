using Application.Common.Models;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Rewards.GetAvailableRewards;

public record GetAvailableRewardsQuery(
    RewardType? Type = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<RewardDto>>>;
