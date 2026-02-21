using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Loyalty.GetMyPointsHistory;

public class GetMyPointsHistoryQueryHandler
    : IRequestHandler<GetMyPointsHistoryQuery, Result<PaginatedList<PointsTransactionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyPointsHistoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<PointsTransactionDto>>> Handle(
        GetMyPointsHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.PointsTransactions
            .AsNoTracking()
            .Where(pt => pt.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var transactions = await query
            .OrderByDescending(pt => pt.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = transactions.Select(pt => new PointsTransactionDto(
            pt.Id, pt.Type, pt.Points, pt.OrderId,
            pt.Description, pt.IsBeeFiBonus, pt.CreatedAt))
            .ToList();

        return Result.Success(new PaginatedList<PointsTransactionDto>(
            dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
