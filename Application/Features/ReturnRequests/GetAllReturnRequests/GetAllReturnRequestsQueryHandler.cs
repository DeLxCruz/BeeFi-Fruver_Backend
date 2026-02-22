using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ReturnRequests.GetAllReturnRequests;

public class GetAllReturnRequestsQueryHandler
    : IRequestHandler<GetAllReturnRequestsQuery, Result<List<AllReturnRequestDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllReturnRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AllReturnRequestDto>>> Handle(
        GetAllReturnRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ReturnRequests.AsNoTracking();

        if (request.Status.HasValue)
            query = query.Where(rr => rr.Status == request.Status.Value);

        var returns = await query
            .OrderByDescending(rr => rr.CreatedAt)
            .Select(rr => new AllReturnRequestDto(
                rr.Id,
                rr.OrderId,
                rr.Order.OrderNumber,
                rr.UserId,
                rr.User.FirstName + " " + rr.User.LastName,
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
