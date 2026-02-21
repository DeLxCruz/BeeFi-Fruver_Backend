using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Loyalty.GetMyLoyaltyPoints;

public class GetMyLoyaltyPointsQueryHandler
    : IRequestHandler<GetMyLoyaltyPointsQuery, Result<LoyaltyPointsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyLoyaltyPointsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<LoyaltyPointsDto>> Handle(
        GetMyLoyaltyPointsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var loyalty = await _context.LoyaltyPoints
            .AsNoTracking()
            .FirstOrDefaultAsync(lp => lp.UserId == userId, cancellationToken);

        if (loyalty is null)
        {
            // El registro se crea en el primer pedido; retornar valores en 0
            return Result.Success(new LoyaltyPointsDto(
                userId, 0, 0, 0, 1, DateTime.UtcNow));
        }

        return Result.Success(new LoyaltyPointsDto(
            loyalty.UserId,
            loyalty.TotalPoints,
            loyalty.AvailablePoints,
            loyalty.RedeemedPoints,
            loyalty.CurrentMultiplier,
            loyalty.LastUpdated));
    }
}
