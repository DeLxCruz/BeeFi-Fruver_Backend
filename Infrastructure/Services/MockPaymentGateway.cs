// TODO: Reemplazar con implementación real de Wompi o PayU
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Implementación temporal de pasarela de pago para entornos de desarrollo.
/// Reemplazar con Wompi o PayU en producción.
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    private readonly ILogger<MockPaymentGateway> _logger;

    public MockPaymentGateway(ILogger<MockPaymentGateway> logger)
    {
        _logger = logger;
    }

    public Task<PaymentInitResult> InitiatePaymentAsync(
        PaymentInitRequest request,
        CancellationToken cancellationToken = default)
    {
        var transactionId = $"MOCK-{Guid.NewGuid()}";
        var redirectUrl = $"https://mock-payment.beefi.app/pay/{transactionId}";

        _logger.LogInformation(
            "[MockPaymentGateway] InitiatePayment | Order: {OrderNumber} | Amount: {Amount} | Method: {Method} | TxId: {TxId}",
            request.OrderNumber, request.Amount, request.PaymentMethod, transactionId);

        return Task.FromResult(new PaymentInitResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            RedirectUrl = redirectUrl
        });
    }

    public Task<PaymentStatusResult> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MockPaymentGateway] GetPaymentStatus | TxId: {TxId}", transactionId);

        return Task.FromResult(new PaymentStatusResult
        {
            IsSuccess = true,
            Status = PaymentStatus.Completed,
            TransactionId = transactionId
        });
    }

    public Task<RefundResult> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var refundId = $"REFUND-MOCK-{Guid.NewGuid()}";

        _logger.LogInformation(
            "[MockPaymentGateway] RefundPayment | TxId: {TxId} | Amount: {Amount} | Reason: {Reason} | RefundId: {RefundId}",
            transactionId, amount, reason, refundId);

        return Task.FromResult(new RefundResult
        {
            IsSuccess = true,
            RefundId = refundId
        });
    }
}
