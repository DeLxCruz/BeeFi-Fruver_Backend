using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.GetCatalogByZone;

public record GetCatalogByZoneQuery(
    Guid ZoneId,
    Guid? CategoryId,
    string? SearchTerm,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<ZoneCatalogDto>>>;
