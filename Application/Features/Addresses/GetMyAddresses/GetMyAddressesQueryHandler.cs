using Application.Common.Interfaces;
using Application.Features.Addresses.Common;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, Result<List<AddressDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyAddressesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AddressDto>>> Handle(
        GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .Include(a => a.Zone)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.CreatedAt)
            .Select(a => new AddressDto(
                a.Id,
                a.ZoneId,
                a.Zone.Name,
                a.Zone.City,
                a.Label,
                a.Street,
                a.HouseNumber,
                a.AdditionalInfo,
                a.Latitude,
                a.Longitude,
                a.IsDefault,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(addresses);
    }
}
