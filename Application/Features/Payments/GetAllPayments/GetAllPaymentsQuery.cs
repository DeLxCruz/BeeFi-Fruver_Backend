using Application.Common.Models;
using Application.Features.Payments.GetPaymentByOrder;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Payments.GetAllPayments;

public record GetAllPaymentsQuery(
    PaymentStatus? Status = null,
    PaymentMethod? Method = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<PaymentDto>>>;
