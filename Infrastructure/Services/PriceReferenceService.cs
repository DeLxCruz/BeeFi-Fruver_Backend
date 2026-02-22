using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class PriceReferenceService : IPriceReferenceService
{
    private readonly ApplicationDbContext _context;
    private const int MinSamples = 10;

    public PriceReferenceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PriceReferenceResult?> GetReferenceAsync(
        string query,
        Guid? zoneId,
        CancellationToken ct)
    {
        var normalized = query.ToLowerInvariant().Trim();

        var reference = await _context.PriceReferences
            .Where(pr =>
                EF.Functions.Like(pr.ProductKey.ToLower(), $"%{normalized}%") &&
                (zoneId == null || pr.ZoneId == zoneId || pr.ZoneId == null))
            .OrderByDescending(pr => pr.ZoneId == zoneId)
            .ThenByDescending(pr => pr.SampleCount)
            .FirstOrDefaultAsync(ct);

        if (reference is null || reference.SampleCount < MinSamples)
            return null;

        return new PriceReferenceResult(
            reference.P25,
            reference.P50,
            reference.P75,
            reference.UnitNorm,
            reference.ComputedAt,
            reference.SampleCount,
            reference.SampleCount >= MinSamples);
    }
}
