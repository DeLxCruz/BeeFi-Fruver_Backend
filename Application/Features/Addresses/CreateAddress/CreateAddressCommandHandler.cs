using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAddressCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var zone = await _context.Zones
            .FirstOrDefaultAsync(z => z.Id == request.ZoneId && z.IsActive, cancellationToken);

        if (zone is null)
            return Result.Failure<Guid>(new Error("Zone.NotFound", "La zona no fue encontrada o no está activa"));

        // If setting as default, remove current default
        if (request.IsDefault)
        {
            var currentDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var addr in currentDefaults)
                addr.RemoveDefault();
        }

        var address = Address.Create(
            userId,
            request.ZoneId,
            request.AliasName,
            request.Street,
            request.HouseNumber,
            request.Neighborhood,
            request.Latitude,
            request.Longitude,
            request.IsDefault);

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(address.Id);
    }
}
