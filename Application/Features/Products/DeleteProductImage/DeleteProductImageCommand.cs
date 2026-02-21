using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.DeleteProductImage;

public record DeleteProductImageCommand(Guid ImageId) : IRequest<Result>;
