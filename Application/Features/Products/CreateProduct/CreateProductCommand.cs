using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    Guid CategoryId,
    string? ImageUrl,
    string UnitOfMeasure) : IRequest<Result<CreateProductResponse>>;
