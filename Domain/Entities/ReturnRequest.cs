using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class ReturnRequest : Entity, IAuditableEntity
{
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? EvidenceUrl { get; private set; }
    public ReturnStatus Status { get; private set; }
    public string? AdminNotes { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public RefundType RefundType { get; private set; }
    public decimal? RefundAmount { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual Order Order { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    private ReturnRequest() { }

    private ReturnRequest(Guid id) : base(id) { }

    public static ReturnRequest Create(
        Guid orderId,
        Guid userId,
        string reason,
        string? evidenceUrl)
    {
        return new ReturnRequest(Guid.NewGuid())
        {
            OrderId = orderId,
            UserId = userId,
            Reason = reason,
            EvidenceUrl = evidenceUrl,
            Status = ReturnStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(Guid adminId, RefundType refundType, decimal? refundAmount, string? notes)
    {
        Status = ReturnStatus.Approved;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        RefundType = refundType;
        RefundAmount = refundAmount;
        AdminNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid adminId, string? notes)
    {
        Status = ReturnStatus.Rejected;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        AdminNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
