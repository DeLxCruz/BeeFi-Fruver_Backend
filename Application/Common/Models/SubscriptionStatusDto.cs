namespace Application.Common.Models;

public record SubscriptionStatusDto(
    bool IsActive,
    DateTime? ExpirationDate
);
