using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.GetCatalogByZone;

public class GetCatalogByZoneQueryHandler
    : IRequestHandler<GetCatalogByZoneQuery, Result<PaginatedList<ZoneCatalogDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCatalogByZoneQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ZoneCatalogDto>>> Handle(
        GetCatalogByZoneQuery request,
        CancellationToken cancellationToken)
    {
        // Verificar que la zona existe
        var zone = await _context.Zones
            .AsNoTracking()
            .FirstOrDefaultAsync(z => z.Id == request.ZoneId && z.IsActive, cancellationToken);

        if (zone is null)
            return Result.Failure<PaginatedList<ZoneCatalogDto>>(ZoneErrors.NotFound);

        // Obtener IDs de fruvers que operan en esta zona
        var fruverIdsInZone = await _context.FruverZones
            .AsNoTracking()
            .Where(fz => fz.ZoneId == request.ZoneId)
            .Select(fz => fz.FruverId)
            .ToListAsync(cancellationToken);

        var query = _context.FruverProducts
            .AsNoTracking()
            .Where(fp =>
                fruverIdsInZone.Contains(fp.FruverId) &&
                fp.IsAvailable &&
                fp.Stock > 0);

        if (request.CategoryId.HasValue)
            query = query.Where(fp => fp.Product.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(fp => fp.Product.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(fp => fp.Fruver.FirstName)
            .ThenBy(fp => fp.Product.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(fp => new ZoneCatalogDto(
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
                fp.Product.UnitOfMeasure,
                zone.Id,
                zone.Name,
                zone.DeliveryBaseCost))
            .ToListAsync(cancellationToken);

        return Result.Success(
            PaginatedList<ZoneCatalogDto>.Create(items, totalCount, request.PageNumber, request.PageSize));
    }
}
