using Domain.Primitives;
using MediatR;

namespace Application.Features.Banners.GetActiveBanners;

public record GetActiveBannersQuery : IRequest<Result<List<BannerDto>>>;
