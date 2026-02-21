namespace Application.Common.Models;

public class RefundResult
{
    public bool IsSuccess { get; init; }
    public string? RefundId { get; init; }
    public string? ErrorMessage { get; init; }
}
