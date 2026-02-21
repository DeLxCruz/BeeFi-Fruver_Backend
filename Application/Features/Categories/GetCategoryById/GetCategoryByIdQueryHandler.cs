using Application.Common.Interfaces;
using Application.Features.Categories.GetCategories;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Where(c => c.Id == request.CategoryId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                ImageUrl = c.IconUrl,
                c.IsActive,
                c.DisplayOrder,
                c.ParentCategoryId,
                SubCategories = c.SubCategories
                    .Where(sc => sc.IsActive)
                    .OrderBy(sc => sc.DisplayOrder)
                    .Select(sc => new
                    {
                        sc.Id,
                        sc.Name,
                        sc.Description,
                        SubImageUrl = sc.IconUrl,
                        sc.IsActive,
                        sc.DisplayOrder,
                        sc.ParentCategoryId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
            return Result.Failure<CategoryDto>(CategoryErrors.NotFound);

        var subCategoryDtos = category.SubCategories
            .Select(sc => new CategoryDto(
                sc.Id,
                sc.Name,
                sc.Description,
                sc.SubImageUrl,
                sc.IsActive,
                sc.DisplayOrder,
                sc.ParentCategoryId,
                new List<CategoryDto>()))
            .ToList();

        return Result.Success(new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.ImageUrl,
            category.IsActive,
            category.DisplayOrder,
            category.ParentCategoryId,
            subCategoryDtos));
    }
}
