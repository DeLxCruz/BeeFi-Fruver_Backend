using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Products.GetProducts;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler
    : IRequestHandler<GetProductsByCategoryQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsByCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(
        GetProductsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        // Verificar que la categoría existe
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            return Result.Failure<PaginatedList<ProductDto>>(CategoryErrors.NotFound);

        // Cargar todos los IDs de categorías para encontrar descendientes
        var allCategories = await _context.Categories
            .AsNoTracking()
            .Select(c => new { c.Id, c.ParentCategoryId })
            .ToListAsync(cancellationToken);

        // Construir conjunto de IDs descendientes en memoria
        var categoryIds = new HashSet<Guid> { request.CategoryId };
        var queue = new Queue<Guid>();
        queue.Enqueue(request.CategoryId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = allCategories
                .Where(c => c.ParentCategoryId == currentId)
                .Select(c => c.Id);

            foreach (var childId in children)
            {
                if (categoryIds.Add(childId))
                    queue.Enqueue(childId);
            }
        }

        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive && categoryIds.Contains(p.CategoryId));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return Result.Success(
            PaginatedList<ProductDto>.Create(items, totalCount, request.PageNumber, request.PageSize));
    }
}
