using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Banners.CreateBanner;

public class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public CreateBannerCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(
        CreateBannerCommand request,
        CancellationToken cancellationToken)
    {
        var banner = Banner.Create(
            title: request.Title,
            imageUrl: request.ImageUrl,
            displayOrder: request.DisplayOrder,
            linkUrl: request.LinkUrl,
            startsAt: request.StartsAt,
            endsAt: request.EndsAt,
            createdBy: _currentUser.UserId);

        _context.Banners.Add(banner);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("banners:active", cancellationToken);

        return Result.Success(banner.Id);
    }
}
