using Domain.Primitives;
using MediatR;

namespace Application.Features.Banners.DeleteBanner;

public record DeleteBannerCommand(Guid BannerId) : IRequest<Result>;
