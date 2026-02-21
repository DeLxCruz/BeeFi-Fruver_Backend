namespace API.Contracts.Reviews;

public record CreateReviewRequest(
    Guid OrderId,
    Guid FruverId,
    int Rating,
    string? Comment = null);
