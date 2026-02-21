using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public DeleteCategoryCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        // Verificar que no tiene productos activos asociados
        var hasActiveProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == request.CategoryId && p.IsActive, cancellationToken);

        if (hasActiveProducts)
            return Result.Failure(CategoryErrors.HasActiveProducts);

        category.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("categories:tree", cancellationToken);

        return Result.Success();
    }
}
