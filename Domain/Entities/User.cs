using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class User : Entity, IAuditableEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool PhoneConfirmed { get; private set; }
    public AccountStatus AccountStatus { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public DateTime? LastLoginAt { get; set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public virtual ICollection<Address> Addresses { get; private set; } = new List<Address>();
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    public virtual ICollection<Order> FruverOrders { get; private set; } = new List<Order>();
    public virtual BeeFiSubscription? BeeFiSubscription { get; private set; }
    public virtual LoyaltyPoints? LoyaltyPoints { get; private set; }
    public virtual ICollection<FruverProduct> FruverProducts { get; private set; } = new List<FruverProduct>();
    public virtual ICollection<FruverZone> FruverZones { get; private set; } = new List<FruverZone>();
    public virtual ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public virtual ICollection<DeviceToken> DeviceTokens { get; private set; } = new List<DeviceToken>();

    // Constructor privado para EF Core
    private User() { }

    private User(Guid id) : base(id) { }

    // Factory method
    public static User Create(
        string email,
        string firstName,
        string lastName,
        string phoneNumber,
        bool requiresApproval = false)
    {
        return new User(Guid.NewGuid())
        {
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            IsActive = true,
            EmailConfirmed = false,
            PhoneConfirmed = false,
            AccountStatus = requiresApproval ? AccountStatus.Pending : AccountStatus.Approved,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Métodos de dominio
    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void ConfirmPhone()
    {
        PhoneConfirmed = true;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetProfileImage(string imageUrl)
    {
        ProfileImageUrl = imageUrl;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Approve(Guid approvedByUserId)
    {
        AccountStatus = AccountStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedByUserId;
        RejectionReason = null;
        IsActive = true;
    }

    public void Reject(string reason, Guid rejectedByUserId)
    {
        AccountStatus = AccountStatus.Rejected;
        RejectionReason = reason;
        ApprovedBy = rejectedByUserId;
        IsActive = false;
    }

    public void Suspend(string reason)
    {
        AccountStatus = AccountStatus.Suspended;
        RejectionReason = reason;
        IsActive = false;
    }

    public bool RequiresApproval() => AccountStatus == AccountStatus.Pending;
    
    public bool IsApproved() => AccountStatus == AccountStatus.Approved;
}