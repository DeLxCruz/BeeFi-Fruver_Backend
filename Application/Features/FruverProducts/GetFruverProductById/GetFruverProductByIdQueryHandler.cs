using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.GetFruverProductById;

public class GetFruverProductByIdQueryHandler
    : IRequestHandler<GetFruverProductByIdQuery, Result<FruverProductDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFruverProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<FruverProductDetailDto>> Handle(
        GetFruverProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var item = await _context.FruverProducts
            .AsNoTracking()
            .Include(fp => fp.Variants.Where(v => v.IsActive).OrderBy(v => v.DisplayOrder))
            .Where(fp => fp.Id == request.FruverProductId)
            .Select(fp => new FruverProductDetailDto(
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
                fp.PreparationTimeMinutes,
                fp.IsSeasonal,
                fp.AvailableFrom,
                fp.AvailableUntil,
                fp.AllowPreOrder,
                fp.PreOrderAvailableDate,
                fp.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.DisplayOrder)
                    .Select(v => new ProductVariantDto(
                        v.Id, v.Name, v.SKU, v.PriceAdjustment,
                        v.Stock, v.IsActive, v.DisplayOrder))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
            return Result.Failure<FruverProductDetailDto>(FruverProductErrors.NotFound);

        return Result.Success(item);
    }
}
