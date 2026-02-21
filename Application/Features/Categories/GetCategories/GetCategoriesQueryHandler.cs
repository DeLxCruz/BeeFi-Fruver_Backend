using Application.Common.Interfaces;
using Application.Features.Categories.GetCategories;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.GetCategories;

public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public const string CacheKey = "categories:tree";

    public GetCategoriesQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<List<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _cache.GetOrCreateAsync(
            CacheKey,
            () => BuildCategoryTreeAsync(cancellationToken),
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result.Success(result);
    }

    private async Task<List<CategoryDto>> BuildCategoryTreeAsync(CancellationToken cancellationToken)
    {
        // Cargar todas las categorías activas en una sola query
        var allCategories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                ImageUrl = c.IconUrl,
                c.IsActive,
                c.DisplayOrder,
                c.ParentCategoryId
            })
            .ToListAsync(cancellationToken);

        // Construir el árbol en memoria
        var lookup = allCategories.ToDictionary(
            c => c.Id,
            c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                c.IsActive,
                c.DisplayOrder,
                c.ParentCategoryId,
                new List<CategoryDto>()));

        var roots = new List<CategoryDto>();

        foreach (var item in allCategories)
        {
            var dto = lookup[item.Id];

            if (item.ParentCategoryId.HasValue && lookup.TryGetValue(item.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories.Add(dto);
            }
            else
            {
                roots.Add(dto);
            }
        }

        return roots;
    }
}
