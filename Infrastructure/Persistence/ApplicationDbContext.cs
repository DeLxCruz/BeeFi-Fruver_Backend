using System.Reflection;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{

    // Authentication & Users
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // BeeFi Service
    public DbSet<BeeFiPlan> BeeFiPlans => Set<BeeFiPlan>();
    public DbSet<BeeFiSubscription> BeeFiSubscriptions => Set<BeeFiSubscription>();
    public DbSet<BeeFiBenefit> BeeFiBenefits => Set<BeeFiBenefit>();
    public DbSet<BeeFiBenefitUsage> BeeFiBenefitUsages => Set<BeeFiBenefitUsage>();

    // Catalog
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<FruverProduct> FruverProducts => Set<FruverProduct>();

    // Location
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<FruverZone> FruverZones => Set<FruverZone>();

    // Orders
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Payments
    public DbSet<Payment> Payments => Set<Payment>();

    // Deliveries
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryStatusHistory> DeliveryStatusHistories => Set<DeliveryStatusHistory>();

    // Gamification
    public DbSet<LoyaltyPoints> LoyaltyPoints => Set<LoyaltyPoints>();
    public DbSet<PointsTransaction> PointsTransactions => Set<PointsTransaction>();
    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<UserReward> UserRewards => Set<UserReward>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Cart
    public DbSet<CartItem> CartItems => Set<CartItem>();

    // CMS
    public DbSet<Banner> Banners => Set<Banner>();

    // Reviews
    public DbSet<Review> Reviews => Set<Review>();

    // Delivery Assignment
    public DbSet<DeliveryPersonZone> DeliveryPersonZones => Set<DeliveryPersonZone>();

    // Comisiones
    public DbSet<CommissionRule> CommissionRules => Set<CommissionRule>();

    // Variantes de producto
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    // Devoluciones
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();

    // Precio de referencia (Anexo A1)
    public DbSet<PriceReference> PriceReferences => Set<PriceReference>();
    public DbSet<SalesAggDaily> SalesAggDaily => Set<SalesAggDaily>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar todas las configuraciones del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Soft delete global filter
        modelBuilder.Entity<Address>().HasQueryFilter(a => !a.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Interceptor para IAuditableEntity
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                // TODO: Obtener usuario actual desde HttpContext
                // entry.Entity.CreatedBy = _currentUserService.UserId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                // TODO: Obtener usuario actual desde HttpContext
                // entry.Entity.UpdatedBy = _currentUserService.UserId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}