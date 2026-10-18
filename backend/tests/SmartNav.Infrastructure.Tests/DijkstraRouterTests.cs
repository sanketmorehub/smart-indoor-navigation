using SmartNav.Navigation;

namespace SmartNav.Infrastructure.Tests;

public sealed class DijkstraRouterTests
{
    [Fact]
    public void FindsLowestTimeRouteAcrossMultipleFloors()
    {
        var start = Guid.NewGuid();
        var junction = Guid.NewGuid();
        var viaLift = Guid.NewGuid();
        var destination = Guid.NewGuid();
        var nodes = new[] { start, junction, viaLift, destination }.Select(id => new RouteNode(id));
        var edges = new[]
        {
            new RouteEdge(start, junction, 4, 4),
            new RouteEdge(junction, destination, 30, 30),
            new RouteEdge(start, viaLift, 8, 8),
            new RouteEdge(viaLift, destination, 12, 12),
        };

        var route = DijkstraRouter.FindShortestPath(nodes, edges, start, destination);

        Assert.NotNull(route);
        Assert.Equal(new[] { start, viaLift, destination }, route.NodeIds);
        Assert.Equal(20m, route.DistanceMeters);
        Assert.Equal(20, route.TravelSeconds);
    }

    [Fact]
    public void AccessibleRouteExcludesStairsAndReturnsNullWhenNoPathExists()
    {
        var start = Guid.NewGuid();
        var destination = Guid.NewGuid();
        var nodes = new[] { new RouteNode(start), new RouteNode(destination) };
        var edges = new[] { new RouteEdge(start, destination, 5, 5, IsAccessible: false, RequiresStairs: true) };

        Assert.Null(DijkstraRouter.FindShortestPath(nodes, edges, start, destination, accessibleOnly: true));
        Assert.NotNull(DijkstraRouter.FindShortestPath(nodes, edges, start, destination));
    }

    [Fact]
    public void TraversesBidirectionalEdgesInEitherDirection()
    {
        var start = Guid.NewGuid();
        var destination = Guid.NewGuid();
        var nodes = new[] { new RouteNode(start), new RouteNode(destination) };
        var edges = new[] { new RouteEdge(destination, start, 5, 5) };

        var route = DijkstraRouter.FindShortestPath(nodes, edges, start, destination);

        Assert.NotNull(route);
        Assert.Equal(new[] { start, destination }, route.NodeIds);
    }
}
