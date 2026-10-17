using SmartNav.Domain.Common;

namespace SmartNav.Domain.Navigation;

/// <summary>A physical lift/staircase/escalator spanning floors. One node per served floor.</summary>
public sealed class VerticalConnector : TenantEntity
{
    public Guid BuildingId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public ConnectorType ConnectorType { get; set; }
    public bool IsAccessible { get; set; }
    /// <summary>Staff-only lifts etc. Never offered to anonymous visitors.</summary>
    public bool IsRestricted { get; set; }
    public ConnectorStatus Status { get; set; } = ConnectorStatus.Active;
    public int? WaitSeconds { get; set; }
    public int? SecondsPerFloor { get; set; }
}

/// <summary>A point in the walkable graph. X/Y are in the floor map's viewBox units.</summary>
public sealed class NavigationNode : TenantEntity
{
    public Guid BuildingId { get; set; }
    public Guid FloorId { get; set; }
    /// <summary>Admin-facing label (e.g. "N101"). Unique per building, never used in public URLs.</summary>
    public required string Code { get; set; }
    public NodeType NodeType { get; set; } = NodeType.Waypoint;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ConnectorId { get; set; }
    public bool IsAccessible { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

/// <summary>A traversable link. Cost in the engine is TIME (seconds); distance is shown to users.</summary>
public sealed class NavigationEdge : TenantEntity
{
    public Guid BuildingId { get; set; }
    public Guid FromNodeId { get; set; }
    public Guid ToNodeId { get; set; }
    public EdgeType EdgeType { get; set; } = EdgeType.Walk;
    public bool IsBidirectional { get; set; } = true;
    public decimal DistanceMeters { get; set; }
    public int? CostOverrideSeconds { get; set; }
    public bool IsAccessible { get; set; } = true;
    public bool RequiresStairs { get; set; }
    public bool IsRestricted { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>Immutable published graph. Visitors only ever route on snapshots, never on draft tables.</summary>
public sealed class GraphSnapshot : TenantEntity
{
    public Guid BuildingId { get; set; }
    public int Version { get; set; }
    public required string SnapshotJson { get; set; }
    public int NodeCount { get; set; }
    public int EdgeCount { get; set; }
    public Guid? PublishedByUserId { get; set; }
    public DateTime PublishedAt { get; set; }
}
