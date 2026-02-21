using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Banners.DeleteBanner;

public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public DeleteBannerCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result> Handle(
        DeleteBannerCommand request,
        CancellationToken cancellationToken)
    {
        var banner = await _context.Banners
            .FirstOrDefaultAsync(b => b.Id == request.BannerId, cancellationToken);

        if (banner is null)
            return Result.Failure(BannerErrors.NotFound);

        _context.Banners.Remove(banner);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("banners:active", cancellationToken);

        return Result.Success();
    }
}
