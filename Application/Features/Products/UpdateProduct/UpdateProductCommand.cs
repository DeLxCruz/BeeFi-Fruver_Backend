using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    Guid CategoryId,
    string? ImageUrl,
    string UnitOfMeasure,
    bool IsActive) : IRequest<Result>;
