using Domain.Primitives;

namespace Domain.Errors;

public static class ReturnRequestErrors
{
    public static readonly Error NotFound =
        new("ReturnRequest.NotFound", "La solicitud de devolución no fue encontrada");

    public static readonly Error OrderNotDelivered =
        new("ReturnRequest.OrderNotDelivered", "Solo se puede solicitar devolución de pedidos entregados");

    public static readonly Error AlreadyExists =
        new("ReturnRequest.AlreadyExists", "Ya existe una solicitud de devolución para este pedido");

    public static readonly Error NotOwner =
        new("ReturnRequest.NotOwner", "No tienes permiso para ver esta solicitud");

    public static readonly Error AlreadyReviewed =
        new("ReturnRequest.AlreadyReviewed", "Esta solicitud ya fue revisada");
}
