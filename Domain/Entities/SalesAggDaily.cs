namespace Domain.Entities;

public class SalesAggDaily
{
    public Guid Id { get; private set; }
    public string ProductKey { get; private set; } = null!;
    public Guid? ZoneId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public DateOnly Date { get; private set; }
    public int UnitsSold { get; private set; }
    public decimal Revenue { get; private set; }
    public int OrderCount { get; private set; }

    private SalesAggDaily() { }

    public static SalesAggDaily Create(
        string productKey,
        DateOnly date,
        int unitsSold,
        decimal revenue,
        int orderCount,
        Guid? zoneId = null,
        Guid? categoryId = null)
    {
        return new SalesAggDaily
        {
            Id = Guid.NewGuid(),
            ProductKey = productKey,
            ZoneId = zoneId,
            CategoryId = categoryId,
            Date = date,
            UnitsSold = unitsSold,
            Revenue = revenue,
            OrderCount = orderCount
        };
    }
}
