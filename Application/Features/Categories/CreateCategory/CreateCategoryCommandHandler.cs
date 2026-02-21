using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateCategoryCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que la categoría padre existe y está activa
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.Categories
                .AnyAsync(
                    c => c.Id == request.ParentCategoryId.Value && c.IsActive,
                    cancellationToken);

            if (!parentExists)
                return Result.Failure<CreateCategoryResponse>(CategoryErrors.ParentNotFound);
        }

        // Verificar nombre único dentro del mismo nivel
        var nameExists = await _context.Categories
            .AnyAsync(
                c => c.Name == request.Name &&
                     c.ParentCategoryId == request.ParentCategoryId,
                cancellationToken);

        if (nameExists)
            return Result.Failure<CreateCategoryResponse>(CategoryErrors.AlreadyExists);

        var category = Category.Create(
            request.Name,
            request.Description ?? string.Empty,
            request.ImageUrl ?? string.Empty,
            request.DisplayOrder,
            request.ParentCategoryId);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("categories:tree", cancellationToken);

        return Result.Success(new CreateCategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.IconUrl,
            category.DisplayOrder,
            category.ParentCategoryId));
    }
}
