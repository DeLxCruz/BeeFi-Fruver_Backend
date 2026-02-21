namespace Domain.Enums;

public enum NotificationType
{
    // --- Pedidos ---
    OrderConfirmed = 0,
    OrderPreparing = 1,
    OrderInProgress = 1,    // Alias semántico de OrderPreparing
    OrderOnDelivery = 2,
    OrderInRoute = 2,       // Alias semántico de OrderOnDelivery
    OrderDelivered = 3,
    OrderCancelled = 4,

    // --- Pagos ---
    PaymentReceived = 5,
    PaymentConfirmed = 5,   // Alias semántico de PaymentReceived
    PromotionAlert = 6,
    BeeFiBenefitAvailable = 7,

    // --- Gamificación ---
    PointsEarned = 8,
    RewardAvailable = 9,
    NewProductAvailable = 10,

    // --- Nuevos valores ---
    PaymentFailed = 11,
    PaymentRefunded = 12,
    RewardExpiringSoon = 13,
    FruverProductLowStock = 14,
    FruverNewOrder = 15,
    AccountApproved = 16,
    AccountRejected = 17,
    AccountSuspended = 18,
    PromotionalMessage = 19,
    SystemAlert = 20

}
