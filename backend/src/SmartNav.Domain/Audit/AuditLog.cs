using SmartNav.Domain.Common;

namespace SmartNav.Domain.Audit;

/// <summary>
/// Append-only audit trail. TenantId is nullable (platform actions), so it is NOT globally
/// filtered - tenant audit queries must always filter by the current tenant explicitly.
/// </summary>
public sealed class AuditLog
{
    public long Id { get; private set; }
    public Guid? TenantId { get; set; }
    public Guid? ActorUserId { get; set; }
    public AuditActorType ActorType { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
}
