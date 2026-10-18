using SmartNav.Domain.Common;

namespace SmartNav.Navigation;

public sealed record PublishedGraph(
    IReadOnlyList<PublishedNode> Nodes,
    IReadOnlyList<PublishedEdge> Edges);

public sealed record PublishedNode(
    Guid Id,
    Guid FloorId,
    string Code,
    NodeType NodeType,
    decimal X,
    decimal Y,
    Guid? LocationId,
    Guid? ConnectorId,
    string? ConnectorName,
    ConnectorType? ConnectorType,
    int? ConnectorWaitSeconds,
    int? ConnectorSecondsPerFloor,
    bool IsAccessible);

public sealed record PublishedEdge(
    Guid FromNodeId,
    Guid ToNodeId,
    EdgeType EdgeType,
    bool IsBidirectional,
    decimal DistanceMeters,
    int? CostOverrideSeconds,
    bool IsAccessible,
    bool RequiresStairs,
    bool IsRestricted);
