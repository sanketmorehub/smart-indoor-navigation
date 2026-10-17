using SmartNav.Domain.Common;

namespace SmartNav.Domain.Tenancy;

/// <summary>An organization using the platform (hospital, hotel, college, mall, office...).</summary>
public sealed class Tenant : Entity, ISoftDeletable
{
    public required string Slug { get; set; }
    public required string Name { get; set; }
    /// <summary>Informational only (HOSPITAL, HOTEL, COLLEGE...). Never drives business logic.</summary>
    public string? OrganizationType { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public string DefaultLocale { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";
    public DateTime? DeletedAt { get; set; }

    public TenantBranding? Branding { get; set; }
    public TenantSettings? Settings { get; set; }
}

/// <summary>1:1 branding used by the visitor app (logo, colours, welcome text).</summary>
public sealed class TenantBranding : ITenantOwned
{
    public Guid TenantId { get; private set; }
    public required string DisplayName { get; set; }
    public Guid? LogoAssetId { get; set; }
    public Guid? FaviconAssetId { get; set; }
    public string PrimaryColor { get; set; } = "#0F62FE";
    public string SecondaryColor { get; set; } = "#393939";
    public string? WelcomeMessage { get; set; }
    public DateTime UpdatedAt { get; private set; }
}

public sealed class TenantSettings : ITenantOwned
{
    public Guid TenantId { get; private set; }
    public string DistanceUnit { get; set; } = "METRIC";
    public decimal DefaultWalkingSpeedMps { get; set; } = 1.2m;
    /// <summary>Rarely-used flags only. Anything queried or validated deserves a real column.</summary>
    public string? SettingsJson { get; set; }
    public DateTime UpdatedAt { get; private set; }
}
