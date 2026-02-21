using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Authentication & Users
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    // BeeFi Service
    DbSet<BeeFiPlan> BeeFiPlans { get; }
    DbSet<BeeFiSubscription> BeeFiSubscriptions { get; }
    DbSet<BeeFiBenefit> BeeFiBenefits { get; }
    DbSet<BeeFiBenefitUsage> BeeFiBenefitUsages { get; }

    // Catalog
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<FruverProduct> FruverProducts { get; }

    // Location
    DbSet<Zone> Zones { get; }
    DbSet<Address> Addresses { get; }
    DbSet<FruverZone> FruverZones { get; }

    // Orders
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    // Payments
    DbSet<Payment> Payments { get; }

    // Deliveries
    DbSet<Delivery> Deliveries { get; }
    DbSet<DeliveryStatusHistory> DeliveryStatusHistories { get; }

    // Gamification
    DbSet<LoyaltyPoints> LoyaltyPoints { get; }
    DbSet<PointsTransaction> PointsTransactions { get; }
    DbSet<Reward> Rewards { get; }
    DbSet<UserReward> UserRewards { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<DeviceToken> DeviceTokens { get; }

    // Audit
    DbSet<AuditLog> AuditLogs { get; }

    // Cart
    DbSet<CartItem> CartItems { get; }

    // CMS
    DbSet<Banner> Banners { get; }

    // Reviews
    DbSet<Review> Reviews { get; }

    // Delivery Assignment
    DbSet<DeliveryPersonZone> DeliveryPersonZones { get; }

    // Método para guardar cambios
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Exponer Database para transacciones (usado en TransactionBehavior)
    DatabaseFacade Database { get; }
}
