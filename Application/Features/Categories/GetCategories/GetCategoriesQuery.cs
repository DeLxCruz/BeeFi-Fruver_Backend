using Domain.Primitives;
using MediatR;

namespace Application.Features.Categories.GetCategories;

public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>;
