using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainPriceReference = Domain.Entities.PriceReference;

namespace Application.Features.PriceReference.RecomputePriceReference;

public class RecomputePriceReferenceCommandHandler
    : IRequestHandler<RecomputePriceReferenceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public RecomputePriceReferenceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(
        RecomputePriceReferenceCommand request,
        CancellationToken cancellationToken)
    {
        var windowStart = DateTime.UtcNow.AddDays(-30);

        var orderItemsQuery = _context.OrderItems
            .AsNoTracking()
            .Where(oi => oi.Order.CreatedAt >= windowStart &&
                         oi.Order.Status == OrderStatus.Delivered);

        if (request.ProductKey is not null)
            orderItemsQuery = orderItemsQuery.Where(
                oi => oi.ProductName.ToLower().Contains(request.ProductKey.ToLower()));

        if (request.ZoneId.HasValue)
            orderItemsQuery = orderItemsQuery.Where(
                oi => oi.Order.Address.ZoneId == request.ZoneId);

        var salesData = await orderItemsQuery
            .Select(oi => new
            {
                ProductKey = oi.ProductName.ToLower().Trim(),
                ZoneId = oi.Order.Address.ZoneId,
                CategoryId = oi.FruverProduct.Product.CategoryId,
                UnitPrice = oi.UnitPrice,
                UnitNorm = oi.FruverProduct.Product.UnitOfMeasure
            })
            .ToListAsync(cancellationToken);

        var grouped = salesData
            .GroupBy(x => new { x.ProductKey, x.ZoneId })
            .ToList();

        var updated = 0;

        foreach (var group in grouped)
        {
            var prices = group.Select(x => x.UnitPrice).OrderBy(p => p).ToList();
            if (prices.Count < 3) continue;

            var p25 = Percentile(prices, 25);
            var p50 = Percentile(prices, 50);
            var p75 = Percentile(prices, 75);
            var categoryId = group.First().CategoryId;
            var unitNorm = group.First().UnitNorm ?? "unidad";

            var existing = await _context.PriceReferences
                .FirstOrDefaultAsync(pr =>
                    pr.ProductKey == group.Key.ProductKey &&
                    pr.ZoneId == group.Key.ZoneId, cancellationToken);

            if (existing is not null)
            {
                existing.Update(p25, p50, p75, prices.Count);
            }
            else
            {
                var newRef = DomainPriceReference.Create(
                    group.Key.ProductKey,
                    p25, p50, p75,
                    unitNorm,
                    prices.Count,
                    30,
                    group.Key.ZoneId,
                    categoryId);
                _context.PriceReferences.Add(newRef);
            }

            updated++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(updated);
    }

    private static decimal Percentile(List<decimal> sortedValues, int percentile)
    {
        if (sortedValues.Count == 0) return 0m;
        var index = (percentile / 100.0) * (sortedValues.Count - 1);
        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);
        if (lower == upper) return sortedValues[lower];
        var fraction = index - lower;
        return sortedValues[lower] + (sortedValues[upper] - sortedValues[lower]) * (decimal)fraction;
    }
}
