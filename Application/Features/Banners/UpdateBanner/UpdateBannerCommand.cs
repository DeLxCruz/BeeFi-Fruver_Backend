using Domain.Primitives;
using MediatR;

namespace Application.Features.Banners.UpdateBanner;

public record UpdateBannerCommand(
    Guid BannerId,
    string Title,
    string ImageUrl,
    string? LinkUrl,
    bool IsActive,
    int DisplayOrder,
    DateTime? StartsAt,
    DateTime? EndsAt) : IRequest<Result>;
