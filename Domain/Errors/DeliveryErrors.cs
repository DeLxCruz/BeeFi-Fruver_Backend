using Domain.Primitives;

namespace Domain.Errors;

public static class DeliveryErrors
{
    public static readonly Error NotFound =
        new("Delivery.NotFound", "La entrega no fue encontrada");

    public static readonly Error AlreadyAssigned =
        new("Delivery.AlreadyAssigned", "Este pedido ya tiene un repartidor asignado");

    public static readonly Error NotAssigned =
        new("Delivery.NotAssigned", "Este pedido no tiene repartidor asignado");

    public static readonly Error NotOwner =
        new("Delivery.NotOwner", "No tienes permiso para actualizar esta entrega");

    public static readonly Error InvalidStatus =
        new("Delivery.InvalidStatus", "La entrega no puede actualizarse en su estado actual");

    public static readonly Error DeliveryPersonNotInZone =
        new("Delivery.DeliveryPersonNotInZone", "El repartidor no está asignado a la zona de este pedido");

    public static readonly Error ZoneAssignmentNotFound =
        new("Delivery.ZoneAssignmentNotFound", "La asignación de zona para este repartidor no fue encontrada");

    public static readonly Error AlreadyInZone =
        new("Delivery.AlreadyInZone", "El repartidor ya está asignado a esta zona");

    public static readonly Error ZoneNotFound =
        new("Delivery.ZoneNotFound", "La zona no fue encontrada o no está activa");

    public static readonly Error DeliveryPersonNotFound =
        new("Delivery.DeliveryPersonNotFound", "El repartidor no fue encontrado o no tiene el rol requerido");

    public static readonly Error InvalidTransition =
        new("Delivery.InvalidTransition", "La transición de estado no es válida");

    public static readonly Error OrderAlreadyHasDelivery =
        new("Delivery.OrderAlreadyHasDelivery", "Este pedido ya tiene una entrega registrada");

    public static readonly Error OrderNotEligible =
        new("Delivery.OrderNotEligible", "El pedido no está en un estado válido para crear una entrega");

    public static readonly Error ProofRequired =
        new("Delivery.ProofRequired", "Se requiere URL de prueba de entrega o PIN para marcar como entregado");
}
