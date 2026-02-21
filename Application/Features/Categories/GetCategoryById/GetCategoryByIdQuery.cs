using Domain.Primitives;
using MediatR;

namespace Application.Features.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid CategoryId) : IRequest<Result<GetCategories.CategoryDto>>;
