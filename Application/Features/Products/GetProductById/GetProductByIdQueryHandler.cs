using Application.Common.Interfaces;
using Application.Features.Products.GetProducts;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.ProductId)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.CategoryId,
                p.Category.Name,
                p.MainImageUrl,
                p.UnitOfMeasure,
                p.IsActive,
                p.CreatedAt,
                p.FruverProducts.Count(fp => fp.IsAvailable && fp.Stock > 0)))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return Result.Failure<ProductDto>(ProductErrors.NotFound);

        return Result.Success(product);
    }
}
