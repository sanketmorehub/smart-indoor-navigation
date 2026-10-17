using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartNav.Application.Abstractions;
using SmartNav.Domain.Audit;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Common;
using SmartNav.Domain.Files;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Navigation;
using SmartNav.Domain.Qr;
using SmartNav.Domain.Tenancy;

namespace SmartNav.Infrastructure.Persistence;

public sealed class SmartNavDbContext(DbContextOptions<SmartNavDbContext> options, ITenantContext tenant)
    : DbContext(options)
{
    /// <summary>
    /// Referenced by global query filters (EF parameterizes it per context instance).
    /// Fail-closed: no tenant => Guid.Empty => tenant-owned queries return no rows.
    /// </summary>
    private Guid CurrentTenantId => tenant.TenantId ?? Guid.Empty;

    // Tenancy & identity
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantBranding> TenantBrandings => Set<TenantBranding>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<FileAsset> FileAssets => Set<FileAsset>();

    // Buildings
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<BuildingSettings> BuildingSettings => Set<BuildingSettings>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<FloorMap> FloorMaps => Set<FloorMap>();
    public DbSet<LocationCategory> LocationCategories => Set<LocationCategory>();
    public DbSet<Location> Locations => Set<Location>();

    // Navigation
    public DbSet<VerticalConnector> VerticalConnectors => Set<VerticalConnector>();
    public DbSet<NavigationNode> NavigationNodes => Set<NavigationNode>();
    public DbSet<NavigationEdge> NavigationEdges => Set<NavigationEdge>();
    public DbSet<GraphSnapshot> GraphSnapshots => Set<GraphSnapshot>();

    // QR & audit
    public DbSet<QrCode> QrCodes => Set<QrCode>();
    public DbSet<QrScanEvent> QrScanEvents => Set<QrScanEvent>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        b.Properties<decimal>().HavePrecision(10, 2);
        b.Properties<string>().HaveMaxLength(200);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartNavDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Enums as VARCHAR strings (no MySQL ENUM => no ALTER TABLE for new values).
            foreach (var p in entityType.GetProperties().Where(p => p.ClrType.IsEnum))
            {
                p.SetProviderClrType(typeof(string));
                p.SetMaxLength(30);
            }

            // We generate UUIDv7 ourselves; EF must never invent key values.
            foreach (var key in entityType.GetProperties().Where(p => p.IsPrimaryKey() && p.ClrType == typeof(Guid)))
                key.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;

            // Soft deletes everywhere: never cascade-delete tenant data.
            foreach (var fk in entityType.GetForeignKeys())
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            ApplyGlobalFilter(modelBuilder, entityType.ClrType);
        }
    }

    /// <summary>Tenant filter (+ soft-delete filter) on every ITenantOwned entity. One combined filter per type.</summary>
    private void ApplyGlobalFilter(ModelBuilder modelBuilder, Type clrType)
    {
        var isTenant = typeof(ITenantOwned).IsAssignableFrom(clrType);
        var isSoft = typeof(ISoftDeletable).IsAssignableFrom(clrType);
        if (!isTenant && !isSoft) return;

        var e = Expression.Parameter(clrType, "e");
        Expression? body = null;

        if (isTenant)
        {
            var tenantProp = Expression.Property(e, nameof(ITenantOwned.TenantId));
            var current = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
            body = Expression.Equal(tenantProp, current);
        }

        if (isSoft)
        {
            var deleted = Expression.Equal(
                Expression.Property(e, nameof(ISoftDeletable.DeletedAt)),
                Expression.Constant(null, typeof(DateTime?)));
            body = body is null ? deleted : Expression.AndAlso(body, deleted);
        }

        modelBuilder.Entity(clrType).HasQueryFilter(Expression.Lambda(body!, e));
    }
}
