using Domain.Primitives;
using MediatR;

namespace Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int DisplayOrder
) : IRequest<Result<CreateCategoryResponse>>;
