using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ReturnRequests.GetMyReturnRequests;

public class GetMyReturnRequestsQueryHandler
    : IRequestHandler<GetMyReturnRequestsQuery, Result<List<ReturnRequestDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyReturnRequestsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<ReturnRequestDto>>> Handle(
        GetMyReturnRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var returns = await _context.ReturnRequests
            .AsNoTracking()
            .Where(rr => rr.UserId == userId)
            .OrderByDescending(rr => rr.CreatedAt)
            .Select(rr => new ReturnRequestDto(
                rr.Id,
                rr.OrderId,
                rr.Order.OrderNumber,
                rr.Reason,
                rr.EvidenceUrl,
                rr.Status,
                rr.AdminNotes,
                rr.RefundType,
                rr.RefundAmount,
                rr.CreatedAt,
                rr.ReviewedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(returns);
    }
}
