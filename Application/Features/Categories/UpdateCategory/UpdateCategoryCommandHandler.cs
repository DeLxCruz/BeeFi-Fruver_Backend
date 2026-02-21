using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        category.Update(
            request.Name,
            request.Description ?? string.Empty,
            request.ImageUrl ?? string.Empty,
            request.DisplayOrder);

        if (request.IsActive)
        {
            category.Activate();
        }
        else
        {
            category.Deactivate();

            // Desactivar subcategorías en cascada
            foreach (var sub in category.SubCategories.Where(sc => sc.IsActive))
            {
                sub.Deactivate();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
