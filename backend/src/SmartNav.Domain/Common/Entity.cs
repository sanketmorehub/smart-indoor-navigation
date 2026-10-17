using SmartNav.Shared;

namespace SmartNav.Domain.Common;

/// <summary>Base for all GUID-keyed entities. Timestamps are stamped by the persistence layer.</summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Ids.New();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}

/// <summary>
/// Marker for rows that belong to exactly one tenant. TenantId is NEVER set by
/// application code or request data: the SaveChanges interceptor stamps it from the
/// server-side tenant context and blocks any later change.
/// </summary>
public interface ITenantOwned
{
    Guid TenantId { get; }
}

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
}

public abstract class TenantEntity : Entity, ITenantOwned
{
    public Guid TenantId { get; private set; }
}
