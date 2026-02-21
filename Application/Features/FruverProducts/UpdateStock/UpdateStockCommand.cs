using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.UpdateStock;

public record UpdateStockCommand(Guid FruverProductId, int NewStock) : IRequest<Result>;
