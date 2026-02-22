namespace Application.Common.Interfaces;

public record PriceReferenceResult(
    decimal P25,
    decimal P50,
    decimal P75,
    string UnitNorm,
    DateTime ComputedAt,
    int SampleCount,
    bool IsReliable);

public interface IPriceReferenceService
{
    Task<PriceReferenceResult?> GetReferenceAsync(
        string query,
        Guid? zoneId,
        CancellationToken ct);
}
