namespace Domain.Entities;

public class PriceReference
{
    public Guid Id { get; private set; }
    public string ProductKey { get; private set; } = null!;
    public Guid? ZoneId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public decimal P25 { get; private set; }
    public decimal P50 { get; private set; }
    public decimal P75 { get; private set; }
    public string UnitNorm { get; private set; } = null!;
    public int SampleCount { get; private set; }
    public DateTime ComputedAt { get; private set; }
    public int WindowDays { get; private set; }

    private PriceReference() { }

    public static PriceReference Create(
        string productKey,
        decimal p25,
        decimal p50,
        decimal p75,
        string unitNorm,
        int sampleCount,
        int windowDays,
        Guid? zoneId = null,
        Guid? categoryId = null)
    {
        return new PriceReference
        {
            Id = Guid.NewGuid(),
            ProductKey = productKey,
            ZoneId = zoneId,
            CategoryId = categoryId,
            P25 = p25,
            P50 = p50,
            P75 = p75,
            UnitNorm = unitNorm,
            SampleCount = sampleCount,
            ComputedAt = DateTime.UtcNow,
            WindowDays = windowDays
        };
    }

    public void Update(decimal p25, decimal p50, decimal p75, int sampleCount)
    {
        P25 = p25;
        P50 = p50;
        P75 = p75;
        SampleCount = sampleCount;
        ComputedAt = DateTime.UtcNow;
    }
}
