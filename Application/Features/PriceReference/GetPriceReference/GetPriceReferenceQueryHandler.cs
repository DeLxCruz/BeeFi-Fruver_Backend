using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;

namespace Application.Features.PriceReference.GetPriceReference;

public class GetPriceReferenceQueryHandler
    : IRequestHandler<GetPriceReferenceQuery, Result<PriceReferenceQueryResult>>
{
    private readonly IPriceReferenceService _priceReferenceService;

    public GetPriceReferenceQueryHandler(IPriceReferenceService priceReferenceService)
    {
        _priceReferenceService = priceReferenceService;
    }

    public async Task<Result<PriceReferenceQueryResult>> Handle(
        GetPriceReferenceQuery request,
        CancellationToken cancellationToken)
    {
        var reference = await _priceReferenceService.GetReferenceAsync(
            request.Query,
            request.ZoneId,
            cancellationToken);

        if (reference is null)
        {
            return Result.Success(new PriceReferenceQueryResult(
                false, null, null, null, null, null, null));
        }

        return Result.Success(new PriceReferenceQueryResult(
            true,
            reference.P25,
            reference.P50,
            reference.P75,
            reference.UnitNorm,
            reference.ComputedAt,
            reference.SampleCount));
    }
}
