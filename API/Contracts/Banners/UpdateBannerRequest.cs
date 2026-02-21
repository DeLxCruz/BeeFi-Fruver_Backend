namespace API.Contracts.Banners;

public record UpdateBannerRequest(
    string Title,
    string ImageUrl,
    string? LinkUrl,
    bool IsActive,
    int DisplayOrder,
    DateTime? StartsAt = null,
    DateTime? EndsAt = null);
