using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Orders.Common;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PaginatedList<OrderSummaryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyOrdersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<OrderSummaryDto>>> Handle(
        GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
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
