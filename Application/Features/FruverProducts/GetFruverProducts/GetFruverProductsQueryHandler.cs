using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.GetFruverProducts;

public class GetFruverProductsQueryHandler
    : IRequestHandler<GetFruverProductsQuery, Result<PaginatedList<FruverProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetFruverProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<FruverProductDto>>> Handle(
        GetFruverProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.FruverProducts
            .AsNoTracking()
            .Where(fp => fp.FruverId == request.FruverId);

        // Filtrar por zona: el fruver debe operar en esa zona
        if (request.ZoneId.HasValue)
        {
            var zoneId = request.ZoneId.Value;
            query = query.Where(fp =>
                _context.FruverZones.Any(fz =>
                    fz.FruverId == fp.FruverId && fz.ZoneId == zoneId));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(fp => fp.Product.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(fp => fp.Product.Name.ToLower().Contains(term));
        }

        if (request.InStockOnly)
            query = query.Where(fp => fp.Stock > 0 && fp.IsAvailable);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(fp => fp.Product.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(fp => new FruverProductDto(
                fp.Id,
                fp.ProductId,
                fp.Product.Name,
                fp.Product.MainImageUrl,
                fp.Product.CategoryId,
                fp.Product.Category.Name,
                fp.FruverId,
                fp.Fruver.FirstName + " " + fp.Fruver.LastName,
                fp.Price,
                fp.Stock,
                fp.IsAvailable,
                fp.DiscountPercentage ?? 0m,
                fp.BeeFiExclusiveDiscount ?? 0m,
                fp.Price
                    * (1m - (fp.DiscountPercentage ?? 0m) / 100m)
                    * (1m - (fp.BeeFiExclusiveDiscount ?? 0m) / 100m),
                fp.Product.UnitOfMeasure))
            .ToListAsync(cancellationToken);

        return Result.Success(
            PaginatedList<FruverProductDto>.Create(items, totalCount, request.PageNumber, request.PageSize));
    }
}
