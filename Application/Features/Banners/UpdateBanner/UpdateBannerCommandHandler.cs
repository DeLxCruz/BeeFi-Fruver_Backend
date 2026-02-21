using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Banners.UpdateBanner;

public class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public UpdateBannerCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result> Handle(
        UpdateBannerCommand request,
        CancellationToken cancellationToken)
    {
        var banner = await _context.Banners
            .FirstOrDefaultAsync(b => b.Id == request.BannerId, cancellationToken);

        if (banner is null)
            return Result.Failure(BannerErrors.NotFound);

        banner.Update(request.Title, request.ImageUrl, request.LinkUrl,
            request.DisplayOrder, request.StartsAt, request.EndsAt);

        if (request.IsActive)
            banner.Activate();
        else
            banner.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("banners:active", cancellationToken);

        return Result.Success();
    }
}
