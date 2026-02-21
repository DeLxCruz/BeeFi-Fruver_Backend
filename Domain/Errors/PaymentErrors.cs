using Domain.Primitives;

namespace Domain.Errors;

public static class PaymentErrors
{
    public static readonly Error NotFound =
        new("Payment.NotFound", "El pago no fue encontrado");

    public static readonly Error AlreadyPaid =
        new("Payment.AlreadyPaid", "Este pedido ya fue pagado");

    public static readonly Error InvalidMethod =
        new("Payment.InvalidMethod", "Método de pago no válido para esta operación");

    public static readonly Error GatewayError =
        new("Payment.GatewayError", "Error al procesar el pago. Intenta nuevamente");

    public static readonly Error CannotRefund =
        new("Payment.CannotRefund", "Solo se pueden reembolsar pagos completados");

    public static readonly Error OrderNotFound =
        new("Payment.OrderNotFound", "El pedido asociado no fue encontrado");
}
