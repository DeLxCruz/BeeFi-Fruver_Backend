using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Orders.Common;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PaginatedList<OrderSummaryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<OrderSummaryDto>>> Handle(
        GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(term) ||
                o.User.Email.ToLower().Contains(term) ||
                o.User.FirstName.ToLower().Contains(term) ||
                o.User.LastName.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(o => o.Items)
                .ThenInclude(i => i.Fruver)
            .ToListAsync(cancellationToken);

        var dtos = orders.Select(o =>
        {
            var fruverNames = o.Items
                .Select(i => $"{i.Fruver.FirstName} {i.Fruver.LastName}")
                .Distinct()
                .ToList();

            return new OrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.Status,
                o.Status.ToString(),
                o.Subtotal,
                o.DeliveryFee,
                o.Discount,
                o.BeeFiDiscount,
                o.Total,
                o.PaymentMethod,
                o.PaymentStatus,
                o.CreatedAt,
                o.Items.Count,
                fruverNames);
        }).ToList();

        return Result.Success(new PaginatedList<OrderSummaryDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
