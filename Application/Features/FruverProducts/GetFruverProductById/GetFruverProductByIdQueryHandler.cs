using Application.Common.Interfaces;
using Application.Features.FruverProducts.GetFruverProducts;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.GetFruverProductById;

public class GetFruverProductByIdQueryHandler
    : IRequestHandler<GetFruverProductByIdQuery, Result<FruverProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFruverProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<FruverProductDto>> Handle(
        GetFruverProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var item = await _context.FruverProducts
            .AsNoTracking()
            .Where(fp => fp.Id == request.FruverProductId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
            return Result.Failure<FruverProductDto>(FruverProductErrors.NotFound);

        return Result.Success(item);
    }
}
