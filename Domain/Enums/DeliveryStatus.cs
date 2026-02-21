namespace Domain.Enums;

public enum DeliveryStatus
{
    Pending = 0,
    Assigned = 1,
    PickedUp = 2,
    OnRoute = 3,
    NearDestination = 4,
    Delivered = 5,
    Failed = 6,
    Returned = 7
}