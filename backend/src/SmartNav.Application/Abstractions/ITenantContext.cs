namespace SmartNav.Application.Abstractions;

/// <summary>
/// The server-resolved tenant for the current request:
///  - admin requests: from the validated JWT `tid` claim (after membership check)
///  - visitor requests: from the resolved QR token / signed visitor context
/// NEVER populated from request bodies, route values or query strings.
/// </summary>
public interface ITenantContext
{
    Guid? TenantId { get; }
}

/// <summary>Write side; only auth middleware, QR resolution and seeding may call this.</summary>
public interface ITenantContextSetter
{
    void SetTenant(Guid tenantId);
    void Clear();
}

public interface IQrTokenGenerator
{
    string NewToken();
}
