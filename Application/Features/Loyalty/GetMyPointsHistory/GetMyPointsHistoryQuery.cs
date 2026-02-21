using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Loyalty.GetMyPointsHistory;

public record GetMyPointsHistoryQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<PointsTransactionDto>>>;
