using SmartNav.Domain.Common;

namespace SmartNav.Domain.Identity;

/// <summary>A global person account. One user can be a member of several tenants.</summary>
public sealed class User : Entity, ISoftDeletable
{
    public required string Email { get; set; }
    public required string NormalizedEmail { get; set; }
    public required string PasswordHash { get; set; }
    public required string DisplayName { get; set; }
    public bool IsPlatformAdmin { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutUntil { get; set; }
    /// <summary>Rotated on password/role change to invalidate issued tokens.</summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime? DeletedAt { get; set; }
}

/// <summary>Role = named set of permissions. TenantId NULL = system template (OWNER/ADMIN/STAFF).</summary>
public sealed class Role : Entity
{
    public Guid? TenantId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsSystem { get; set; }
    public List<RolePermission> Permissions { get; set; } = [];
}

public sealed class Permission : Entity
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public PermissionScope Scope { get; set; } = PermissionScope.Tenant;
}

public sealed class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}

/// <summary>Links a user to a tenant with a role. The ONLY source of tenant access for admins.</summary>
public sealed class TenantMembership : TenantEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public Guid? InvitedByUserId { get; set; }
    public User? User { get; set; }
    public Role? Role { get; set; }
}

/// <summary>Only a SHA-256 hash of the refresh token is stored. Rotation uses FamilyId for reuse detection.</summary>
public sealed class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public Guid FamilyId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public string? CreatedIp { get; set; }
    public string? UserAgent { get; set; }
}
