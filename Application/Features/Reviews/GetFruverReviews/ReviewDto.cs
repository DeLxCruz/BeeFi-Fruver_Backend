namespace Application.Features.Reviews.GetFruverReviews;

public record ReviewDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid FruverId,
    Guid OrderId,
    int Rating,
    string? Comment,
    DateTime CreatedAt);
