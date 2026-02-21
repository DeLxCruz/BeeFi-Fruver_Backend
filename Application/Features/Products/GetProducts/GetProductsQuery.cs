using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.GetProducts;

public record GetProductsQuery(
    Guid? CategoryId,
    string? SearchTerm,
    bool? IsActive,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<ProductDto>>>
{
    public GetProductsQuery() : this(null, null, true, 1, 20) { }
}
