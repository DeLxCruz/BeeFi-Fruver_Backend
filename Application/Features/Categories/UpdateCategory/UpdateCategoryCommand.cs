using Domain.Primitives;
using MediatR;

namespace Application.Features.Categories.UpdateCategory;

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsActive,
    int DisplayOrder
) : IRequest<Result>;
