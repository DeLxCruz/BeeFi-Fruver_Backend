using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.DeleteVariant;

public record DeleteVariantCommand(Guid VariantId) : IRequest<Result>;
