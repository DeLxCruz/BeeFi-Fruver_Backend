using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.GetZones;

public record GetZonesQuery : IRequest<Result<List<ZoneDto>>>;
