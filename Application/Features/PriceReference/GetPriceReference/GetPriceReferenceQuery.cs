using Domain.Primitives;
using MediatR;

namespace Application.Features.PriceReference.GetPriceReference;

public record GetPriceReferenceQuery(
    string Query,
    Guid? ZoneId = null) : IRequest<Result<PriceReferenceQueryResult>>;

public record PriceReferenceQueryResult(
    bool IsAvailable,
    decimal? P25,
    decimal? P50,
    decimal? P75,
    string? UnitNorm,
    DateTime? ComputedAt,
    int? SampleCount);
