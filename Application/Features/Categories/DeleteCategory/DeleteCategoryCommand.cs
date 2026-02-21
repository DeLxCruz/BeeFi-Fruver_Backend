using Domain.Primitives;
using MediatR;

namespace Application.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid CategoryId) : IRequest<Result>;
