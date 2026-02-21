using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        // Verificar que no tiene FruverProducts activos
        var hasActiveFruverProducts = await _context.FruverProducts
            .AnyAsync(fp => fp.ProductId == request.ProductId && fp.IsAvailable, cancellationToken);

        if (hasActiveFruverProducts)
            return Result.Failure(new Domain.Primitives.Error(
                "Product.HasActiveFruverProducts",
                "No se puede desactivar un producto que tiene publicaciones activas de fruvers"));

        product.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
