using Domain.Primitives;

namespace Domain.Errors;

public static class ReviewErrors
{
    public static readonly Error NotFound =
        new("Review.NotFound", "La reseña no fue encontrada");

    public static readonly Error AlreadyReviewed =
        new("Review.AlreadyReviewed", "Ya dejaste una reseña para este pedido");

    public static readonly Error OrderNotDelivered =
        new("Review.OrderNotDelivered", "Solo puedes reseñar pedidos entregados");

    public static readonly Error NotOwner =
        new("Review.NotOwner", "No tienes permiso para modificar esta reseña");
}
