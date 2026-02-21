using Domain.Primitives;
using MediatR;

namespace Application.Features.Banners.CreateBanner;

public record CreateBannerCommand(
    string Title,
    string ImageUrl,
    string? LinkUrl,
    int DisplayOrder,
    DateTime? StartsAt,
    DateTime? EndsAt) : IRequest<Result<Guid>>;
