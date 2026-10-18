namespace SmartNav.Navigation;

public sealed record RouteNode(Guid Id, bool IsAccessible = true);

public sealed record RouteEdge(
    Guid FromNodeId,
    Guid ToNodeId,
    decimal DistanceMeters,
    double TravelSeconds,
    bool IsAccessible = true,
    bool IsRestricted = false,
    bool RequiresStairs = false,
    bool IsBidirectional = true);

public sealed record RoutePath(IReadOnlyList<Guid> NodeIds, decimal DistanceMeters, double TravelSeconds);

public static class DijkstraRouter
{
    public static RoutePath? FindShortestPath(
        IEnumerable<RouteNode> nodes,
        IEnumerable<RouteEdge> edges,
        Guid startNodeId,
        Guid destinationNodeId,
        bool accessibleOnly = false)
    {
        var nodeMap = nodes.ToDictionary(node => node.Id);
        if (!nodeMap.ContainsKey(startNodeId) || !nodeMap.ContainsKey(destinationNodeId))
            return null;
        if (accessibleOnly && (!nodeMap[startNodeId].IsAccessible || !nodeMap[destinationNodeId].IsAccessible))
            return null;

        var adjacency = nodeMap.Keys.ToDictionary(id => id, _ => new List<RouteEdge>());
        foreach (var edge in edges)
        {
            if (!nodeMap.ContainsKey(edge.FromNodeId) || !nodeMap.ContainsKey(edge.ToNodeId))
                throw new ArgumentException("Every route edge must connect two nodes in the graph.", nameof(edges));
            if (edge.DistanceMeters < 0 || edge.TravelSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(edges), "Route edge costs cannot be negative.");

            if (IsUsable(edge, accessibleOnly))
            {
                if (accessibleOnly &&
                    (!nodeMap[edge.FromNodeId].IsAccessible || !nodeMap[edge.ToNodeId].IsAccessible))
                    continue;

                adjacency[edge.FromNodeId].Add(edge);
                if (edge.IsBidirectional)
                    adjacency[edge.ToNodeId].Add(edge with { FromNodeId = edge.ToNodeId, ToNodeId = edge.FromNodeId });
            }
        }

        var distances = nodeMap.Keys.ToDictionary(id => id, _ => double.PositiveInfinity);
        var previous = new Dictionary<Guid, (Guid NodeId, RouteEdge Edge)>();
        var queue = new PriorityQueue<Guid, double>();
        distances[startNodeId] = 0;
        queue.Enqueue(startNodeId, 0);

        while (queue.TryDequeue(out var current, out var queuedDistance))
        {
            if (queuedDistance > distances[current])
                continue;
            if (current == destinationNodeId)
                break;

            foreach (var edge in adjacency[current])
            {
                var candidate = distances[current] + edge.TravelSeconds;
                if (candidate >= distances[edge.ToNodeId])
                    continue;

                distances[edge.ToNodeId] = candidate;
                previous[edge.ToNodeId] = (current, edge);
                queue.Enqueue(edge.ToNodeId, candidate);
            }
        }

        if (double.IsPositiveInfinity(distances[destinationNodeId]))
            return null;

        var path = new List<Guid> { destinationNodeId };
        var totalDistance = 0m;
        var cursor = destinationNodeId;
        while (cursor != startNodeId)
        {
            var (parent, edge) = previous[cursor];
            totalDistance += edge.DistanceMeters;
            cursor = parent;
            path.Add(cursor);
        }

        path.Reverse();
        return new RoutePath(path, totalDistance, distances[destinationNodeId]);
    }

    private static bool IsUsable(RouteEdge edge, bool accessibleOnly) =>
        !edge.IsRestricted &&
        (!accessibleOnly || edge.IsAccessible && !edge.RequiresStairs);
}
