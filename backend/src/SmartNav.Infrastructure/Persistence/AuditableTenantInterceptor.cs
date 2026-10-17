using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartNav.Application.Abstractions;
using SmartNav.Domain.Common;
using SmartNav.Shared;

namespace SmartNav.Infrastructure.Persistence;

/// <summary>
/// Enforces tenant ownership on write:
///  - Added ITenantOwned rows get TenantId from the server-side tenant context (never from input)
///  - Writing without a tenant context, or for a different tenant, throws
///  - Changing TenantId on an existing row throws
/// Also stamps CreatedAt/UpdatedAt in UTC.
/// </summary>
public sealed class AuditableTenantInterceptor(ITenantContext tenant, IClock clock) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;
        var now = clock.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;

            if (entry.Entity is ITenantOwned) EnforceTenant(entry);

            if (entry.State == EntityState.Added) SetIfExists(entry, nameof(Entity.CreatedAt), now);
            SetIfExists(entry, nameof(Entity.UpdatedAt), now);
        }
    }

    private void EnforceTenant(EntityEntry entry)
    {
        var current = tenant.TenantId
            ?? throw new InvalidOperationException(
                $"Cannot write {entry.Metadata.ClrType.Name} without a tenant context.");

        var prop = entry.Property(nameof(ITenantOwned.TenantId));

        if (entry.State == EntityState.Added)
        {
            var value = (Guid)prop.CurrentValue!;
            if (value != Guid.Empty && value != current)
                throw new InvalidOperationException("Cross-tenant write blocked.");
            prop.CurrentValue = current;
        }
        else if (prop.IsModified && !Equals(prop.OriginalValue, prop.CurrentValue))
        {
            throw new InvalidOperationException("TenantId is immutable.");
        }
    }

    private static void SetIfExists(EntityEntry entry, string name, DateTime value)
    {
        var p = entry.Metadata.FindProperty(name);
        if (p is not null) entry.Property(name).CurrentValue = value;
    }
}
