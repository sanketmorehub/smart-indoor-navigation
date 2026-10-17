using SmartNav.Application.Abstractions;

namespace SmartNav.Infrastructure.Tenancy;

/// <summary>Scoped per request. Fail-closed: when no tenant is set, tenant-owned queries return nothing.</summary>
public sealed class TenantContext : ITenantContext, ITenantContextSetter
{
    public Guid? TenantId { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        if (TenantId is not null && TenantId != tenantId)
            throw new InvalidOperationException("Tenant context is already set for this scope and cannot be switched.");
        TenantId = tenantId;
    }

    public void Clear() => TenantId = null;
}
