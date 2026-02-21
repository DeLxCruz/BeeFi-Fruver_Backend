namespace API.Contracts.Banners;

public record CreateBannerRequest(
    string Title,
    string ImageUrl,
    string? LinkUrl,
    int DisplayOrder,
    DateTime? StartsAt = null,
    DateTime? EndsAt = null);
