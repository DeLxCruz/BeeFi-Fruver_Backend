using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Loyalty.EarnPoints;

public class EarnPointsCommandHandler : IRequestHandler<EarnPointsCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public EarnPointsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        EarnPointsCommand request,
        CancellationToken cancellationToken)
    {
        var loyalty = await _context.LoyaltyPoints
            .FirstOrDefaultAsync(lp => lp.UserId == request.UserId, cancellationToken);

        if (loyalty is null)
        {
            loyalty = LoyaltyPoints.Create(request.UserId);
            _context.LoyaltyPoints.Add(loyalty);
        }

        // 1 punto por cada $1000 COP gastados
        var basePoints = (int)(request.OrderTotal / 1000);
        var totalPoints = basePoints * loyalty.CurrentMultiplier;

        if (totalPoints <= 0)
            return Result.Success();

        loyalty.AddPoints(totalPoints);

        var transaction = PointsTransaction.Create(
            userId: request.UserId,
            points: totalPoints,
            type: PointsTransactionType.Earned,
            description: $"Puntos ganados por pedido (${request.OrderTotal:N0} COP)",
            orderId: request.OrderId);
        _context.PointsTransactions.Add(transaction);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
