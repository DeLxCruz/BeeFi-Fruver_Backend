namespace API.Contracts.Pricing;

public record RecomputePriceReferenceRequest(
    string? ProductKey = null,
    Guid? ZoneId = null);
