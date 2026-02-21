using Application.Features.Zones.GetZones;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.GetZoneById;

public record GetZoneByIdQuery(Guid ZoneId) : IRequest<Result<ZoneDto>>;
