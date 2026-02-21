using Domain.Primitives;

namespace Domain.Errors;

public static class OrderErrors
{
    public static readonly Error NotFound =
        new("Order.NotFound", "El pedido no fue encontrado");

    public static readonly Error NotOwner =
        new("Order.NotOwner", "No tienes permiso para ver este pedido");

    public static readonly Error InvalidStatus =
        new("Order.InvalidStatus", "El pedido no puede ser modificado en su estado actual");

    public static readonly Error CannotCancel =
        new("Order.CannotCancel", "Solo se pueden cancelar pedidos en estado Pendiente o Confirmado");

    public static readonly Error AddressNotFound =
        new("Order.AddressNotFound", "La dirección de entrega no fue encontrada");

    public static readonly Error AddressNotOwner =
        new("Order.AddressNotOwner", "La dirección no pertenece al usuario");
}
