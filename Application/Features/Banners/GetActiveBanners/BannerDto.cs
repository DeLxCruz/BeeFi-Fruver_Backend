namespace Application.Features.Banners.GetActiveBanners;

public record BannerDto(
    Guid Id,
    string Title,
    string ImageUrl,
    string? LinkUrl,
    int DisplayOrder);
