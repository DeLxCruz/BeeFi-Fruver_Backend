using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Payments.GetPaymentByOrder;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.GetAllPayments;

public class GetAllPaymentsQueryHandler
    : IRequestHandler<GetAllPaymentsQuery, Result<PaginatedList<PaymentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPaymentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<PaymentDto>>> Handle(
        GetAllPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Include(p => p.Order)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(p => p.Status == request.Status.Value);

        if (request.Method.HasValue)
            query = query.Where(p => p.Method == request.Method.Value);

        if (request.FromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.CreatedAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var payments = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PaymentDto(
                p.Id,
                p.OrderId,
                p.Order.OrderNumber,
                p.Method,
                p.Status,
                p.Amount,
                p.TransactionId,
                p.GatewayResponse,
                p.RefundAmount,
                p.RefundedAt,
                p.RefundReason,
                p.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(new PaginatedList<PaymentDto>(payments, totalCount, request.PageNumber, request.PageSize));
    }
}
