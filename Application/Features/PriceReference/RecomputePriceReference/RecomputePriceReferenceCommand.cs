using Domain.Primitives;
using MediatR;

namespace Application.Features.PriceReference.RecomputePriceReference;

public record RecomputePriceReferenceCommand(
    string? ProductKey = null,
    Guid? ZoneId = null) : IRequest<Result<int>>;
