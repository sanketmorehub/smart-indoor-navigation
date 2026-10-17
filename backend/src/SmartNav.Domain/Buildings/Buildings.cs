using SmartNav.Domain.Common;

namespace SmartNav.Domain.Buildings;

public sealed class Building : TenantEntity, ISoftDeletable
{
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public BuildingStatus Status { get; set; } = BuildingStatus.Draft;
    /// <summary>Points to the live GraphSnapshot version. Rollback = re-point.</summary>
    public int? PublishedVersion { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public BuildingSettings? Settings { get; set; }
    public List<Floor> Floors { get; set; } = [];
}

public sealed class BuildingSettings : ITenantOwned
{
    public Guid BuildingId { get; set; }
    public Guid TenantId { get; private set; }
    public decimal WalkingSpeedMps { get; set; } = 1.2m;
    public int DefaultLiftWaitSeconds { get; set; } = 45;
    public int StairSecondsPerFloor { get; set; } = 15;
    public bool AllowManualStartSelection { get; set; } = true;
    public DateTime UpdatedAt { get; private set; }
}

public sealed class Floor : TenantEntity, ISoftDeletable
{
    public Guid BuildingId { get; set; }
    /// <summary>-1 basement, 0 ground, 1 first ... Used for ordering and vertical cost.</summary>
    public int LevelIndex { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal ElevationMeters { get; set; }
    public bool IsPublic { get; set; } = true;
    public RecordStatus Status { get; set; } = RecordStatus.Active;
    public DateTime? DeletedAt { get; set; }

    public Building? Building { get; set; }
}

/// <summary>Versioned SVG background for a floor. Node X/Y use this map's viewBox units.</summary>
public sealed class FloorMap : TenantEntity
{
    public Guid BuildingId { get; set; }
    public Guid FloorId { get; set; }
    public Guid FileAssetId { get; set; }
    public int Version { get; set; }
    public decimal ViewBoxMinX { get; set; }
    public decimal ViewBoxMinY { get; set; }
    public decimal ViewBoxWidth { get; set; }
    public decimal ViewBoxHeight { get; set; }
    /// <summary>Scale used to auto-calculate edge distances from coordinates.</summary>
    public decimal MetersPerUnit { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>Configurable category. TenantId NULL = global default (RECEPTION, LIFT, CLASSROOM...).</summary>
public sealed class LocationCategory : Entity
{
    public Guid? TenantId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsSearchableDefault { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>A searchable place (Cardiology, Room 204, Food Court...). Reached via one or more access nodes.</summary>
public sealed class Location : TenantEntity, ISoftDeletable
{
    public Guid BuildingId { get; set; }
    public Guid FloorId { get; set; }
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public string? ShortCode { get; set; }
    public string? Description { get; set; }
    /// <summary>Comma-separated search synonyms, e.g. "pharmacy, chemist, medicines".</summary>
    public string? Keywords { get; set; }
    public decimal? LabelX { get; set; }
    public decimal? LabelY { get; set; }
    public string? ShapeJson { get; set; }
    public bool IsSearchable { get; set; } = true;
    public bool IsPublic { get; set; } = true;
    public RecordStatus Status { get; set; } = RecordStatus.Active;
    public DateTime? DeletedAt { get; set; }

    public Floor? Floor { get; set; }
    public LocationCategory? Category { get; set; }
}
