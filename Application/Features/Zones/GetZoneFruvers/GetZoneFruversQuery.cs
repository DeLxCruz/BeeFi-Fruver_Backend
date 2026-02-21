using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.GetZoneFruvers;

public record GetZoneFruversQuery(Guid ZoneId) : IRequest<Result<List<ZoneFruverDto>>>;
