using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.UpdateAddress;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAddressCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && !a.IsDeleted, cancellationToken);

        if (address is null)
            return Result.Failure(new Error("Address.NotFound", "La dirección no fue encontrada"));

        if (address.UserId != userId)
            return Result.Failure(new Error("Address.NotOwner", "No tienes permiso para modificar esta dirección"));

        // Handle default flag changes
        if (request.IsDefault && !address.IsDefault)
        {
            var currentDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted && a.Id != request.AddressId)
                .ToListAsync(cancellationToken);

            foreach (var addr in currentDefaults)
                addr.RemoveDefault();

            address.SetAsDefault();
        }
        else if (!request.IsDefault && address.IsDefault)
        {
            address.RemoveDefault();
        }

        address.Update(request.AliasName, request.Street, request.HouseNumber, request.Neighborhood, address.ZoneId);

        if (request.Latitude.HasValue && request.Longitude.HasValue)
            address.UpdateCoordinates(request.Latitude.Value, request.Longitude.Value);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
