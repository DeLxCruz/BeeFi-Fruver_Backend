using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Banners.GetActiveBanners;

public class GetActiveBannersQueryHandler : IRequestHandler<GetActiveBannersQuery, Result<List<BannerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public const string CacheKey = "banners:active";

    public GetActiveBannersQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<List<BannerDto>>> Handle(
        GetActiveBannersQuery request,
        CancellationToken cancellationToken)
    {
        var banners = await _cache.GetOrCreateAsync(
            CacheKey,
            async () =>
            {
                var now = DateTime.UtcNow;
                return await _context.Banners
                    .AsNoTracking()
                    .Where(b => b.IsActive
                        && (b.StartsAt == null || b.StartsAt <= now)
                        && (b.EndsAt == null || b.EndsAt >= now))
                    .OrderBy(b => b.DisplayOrder)
                    .Take(10)
                    .Select(b => new BannerDto(b.Id, b.Title, b.ImageUrl, b.LinkUrl, b.DisplayOrder))
                    .ToListAsync(cancellationToken);
            },
            TimeSpan.FromMinutes(10),
            cancellationToken);

        return Result.Success(banners);
    }
}
