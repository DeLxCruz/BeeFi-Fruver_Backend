using Application.Common.Models;
using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IPaymentGateway
{
    // Inicia un pago y retorna URL de redirección o token
    Task<PaymentInitResult> InitiatePaymentAsync(
        PaymentInitRequest request,
        CancellationToken cancellationToken = default);

    // Verifica el estado de un pago en la pasarela
    Task<PaymentStatusResult> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default);

    // Procesa un reembolso
    Task<RefundResult> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default);
}
