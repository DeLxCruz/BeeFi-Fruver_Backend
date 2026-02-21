using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        // Verificar nueva categoría si cambió
        if (product.CategoryId != request.CategoryId)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId && c.IsActive, cancellationToken);

            if (!categoryExists)
                return Result.Failure(CategoryErrors.NotFound);
        }

        product.Update(
            request.Name,
            request.Description ?? string.Empty,
            request.CategoryId,
            request.ImageUrl ?? product.MainImageUrl,
            request.UnitOfMeasure);

        if (request.IsActive)
            product.Activate();
        else
            product.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
