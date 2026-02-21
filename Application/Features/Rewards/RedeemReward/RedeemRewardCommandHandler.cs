using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Rewards.RedeemReward;

public class RedeemRewardCommandHandler
    : IRequestHandler<RedeemRewardCommand, Result<RedeemRewardResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RedeemRewardCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<RedeemRewardResponse>> Handle(
        RedeemRewardCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var reward = await _context.Rewards
            .FirstOrDefaultAsync(r => r.Id == request.RewardId, cancellationToken);

        if (reward is null)
            return Result.Failure<RedeemRewardResponse>(RewardErrors.NotFound);

        if (!reward.IsActive)
            return Result.Failure<RedeemRewardResponse>(RewardErrors.NotAvailable);

        if (reward.ExpirationDate.HasValue && reward.ExpirationDate < DateTime.UtcNow)
            return Result.Failure<RedeemRewardResponse>(RewardErrors.Expired);

        var loyalty = await _context.LoyaltyPoints
            .FirstOrDefaultAsync(lp => lp.UserId == userId, cancellationToken);

        if (loyalty is null || loyalty.AvailablePoints < reward.PointsRequired)
            return Result.Failure<RedeemRewardResponse>(LoyaltyErrors.InsufficientPoints);

        // Verificar MaxRedemptionsPerUser
        var redemptionCount = await _context.UserRewards
            .CountAsync(ur => ur.UserId == userId && ur.RewardId == reward.Id, cancellationToken);

        if (redemptionCount >= reward.MaxRedemptionsPerUser)
            return Result.Failure<RedeemRewardResponse>(RewardErrors.MaxRedemptionsReached);

        // Descontar puntos
        loyalty.RedeemPoints(reward.PointsRequired);

        // Crear UserReward
        var expirationDate = DateTime.UtcNow.AddDays(30);
        var userReward = UserReward.Create(userId, reward.Id, expirationDate);
        _context.UserRewards.Add(userReward);

        // Crear transacción de puntos (negativa)
        var transaction = PointsTransaction.Create(
            userId: userId,
            points: -reward.PointsRequired,
            type: PointsTransactionType.Redeemed,
            description: $"Canje de recompensa: {reward.Name}");
        _context.PointsTransactions.Add(transaction);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new RedeemRewardResponse(
            userReward.Id,
            reward.Name,
            reward.Value,
            expirationDate));
    }
}
