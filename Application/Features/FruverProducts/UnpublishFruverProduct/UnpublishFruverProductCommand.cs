using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.UnpublishFruverProduct;

public record UnpublishFruverProductCommand(Guid FruverProductId) : IRequest<Result>;
