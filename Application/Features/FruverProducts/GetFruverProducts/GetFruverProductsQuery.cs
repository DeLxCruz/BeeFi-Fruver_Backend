using Application.Common.Models;
using Application.Features.FruverProducts.GetFruverProducts;
using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.GetFruverProducts;

public record GetFruverProductsQuery(
    Guid FruverId,
    Guid? ZoneId,
    Guid? CategoryId,
    string? SearchTerm,
    bool InStockOnly,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<FruverProductDto>>>;
