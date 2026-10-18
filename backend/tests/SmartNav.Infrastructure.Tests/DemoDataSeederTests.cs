using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Common;
using SmartNav.Domain.Navigation;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Infrastructure.Qr;
using SmartNav.Infrastructure.Seeding;
using SmartNav.Navigation;

namespace SmartNav.Infrastructure.Tests;

public sealed class DemoDataSeederTests
{
    [Fact]
    public async Task Seeds_and_publishes_an_idempotent_graph_snapshot()
    {
        using var testDb = new TestDb();
        using var db = testDb.NewContext(out var tenant);
        var seeder = new DemoDataSeeder(
            db,
            tenant,
            new QrTokenGenerator(),
            new ConfigurationBuilder().Build(),
            NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync();

        var building = Assert.Single(db.Buildings);
        Assert.Equal(BuildingStatus.Published, building.Status);
        Assert.Equal(1, building.PublishedVersion);

        var snapshot = Assert.Single(db.GraphSnapshots);
        var graph = JsonSerializer.Deserialize<PublishedGraph>(snapshot.SnapshotJson);
        Assert.NotNull(graph);
        Assert.Equal(12, await db.Floors.CountAsync());
        Assert.True(await db.Locations.CountAsync() >= 80);
        Assert.Equal(await db.Locations.CountAsync(), await db.NavigationNodes.CountAsync(node => node.LocationId != null && node.IsActive));
        Assert.Equal(await db.NavigationNodes.CountAsync(node => node.IsActive), graph.Nodes.Count);
        Assert.Equal(await db.NavigationEdges.CountAsync(edge => edge.IsActive), graph.Edges.Count);
        Assert.Contains(graph.Edges, edge => edge.EdgeType == EdgeType.Lift);
        Assert.Contains(graph.Edges, edge => edge.EdgeType == EdgeType.Stair);
        Assert.Contains(db.Floors, floor => floor.Code == "B2");
        Assert.Contains(db.Floors, floor => floor.Code == "9");
        Assert.Contains(db.Locations, location => location.Name == "Radiation Oncology");
        Assert.Contains(db.Locations, location => location.Name == "International Patient Services");

        var reception = await db.Locations.SingleAsync(location => location.Name == "Reception");
        var startNodeId = graph.Nodes.Single(node => node.LocationId == reception.Id).Id;
        var routeNodes = graph.Nodes.Select(node => new RouteNode(node.Id, node.IsAccessible)).ToArray();
        var routeEdges = graph.Edges.Select(edge => new RouteEdge(
            edge.FromNodeId, edge.ToNodeId, edge.DistanceMeters, 1, edge.IsAccessible,
            edge.IsRestricted, edge.RequiresStairs, edge.IsBidirectional)).ToArray();
        foreach (var destination in await db.Locations.ToListAsync())
        {
            var destinationNodeId = graph.Nodes.Single(node => node.LocationId == destination.Id).Id;
            Assert.NotNull(DijkstraRouter.FindShortestPath(
                routeNodes, routeEdges, startNodeId, destinationNodeId, accessibleOnly: true));
        }

        await seeder.SeedAsync();
        Assert.Single(db.GraphSnapshots);
        Assert.Equal(12, await db.Floors.CountAsync());
        Assert.Equal(await db.Locations.CountAsync(), await db.NavigationNodes.CountAsync(node => node.LocationId != null && node.IsActive));
        Assert.Equal(await db.NavigationNodes.CountAsync(node => node.IsActive), graph.Nodes.Count);
    }
}
