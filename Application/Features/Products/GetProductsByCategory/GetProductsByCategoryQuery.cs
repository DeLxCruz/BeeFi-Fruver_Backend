using Application.Common.Models;
using Application.Features.Products.GetProducts;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.GetProductsByCategory;

public record GetProductsByCategoryQuery(
    Guid CategoryId,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<ProductDto>>>;
