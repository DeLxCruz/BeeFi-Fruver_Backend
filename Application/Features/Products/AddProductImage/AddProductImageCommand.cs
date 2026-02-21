using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.AddProductImage;

public record AddProductImageCommand(
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder) : IRequest<Result>;
