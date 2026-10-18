using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartNav.Application.Abstractions;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Common;
using SmartNav.Domain.Navigation;
using SmartNav.Domain.Qr;
using SmartNav.Domain.Tenancy;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Navigation;

namespace SmartNav.Api.Endpoints;

public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapPublicEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api/public").WithTags("Visitor navigation");
        api.MapGet("/buildings/{tenantSlug}", GetBuildingAsync)
            .Produces<BuildingResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        api.MapGet("/qr/{token}", ResolveQrAsync)
            .Produces<ResolvedQrResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        api.MapPost("/buildings/{tenantSlug}/routes", FindRouteAsync)
            .Produces<RouteResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        return endpoints;
    }

    private static async Task<IResult> GetBuildingAsync(
        string tenantSlug,
        SmartNavDbContext db,
        ITenantContextSetter tenantSetter,
        CancellationToken cancellationToken)
    {
        var tenant = await FindActiveTenantAsync(db, tenantSlug, cancellationToken);
        if (tenant is null)
            return Results.NotFound();

        tenantSetter.SetTenant(tenant.Id);
        var building = await FindPublishedBuildingAsync(db, cancellationToken);
        if (building is null)
            return Results.NotFound();

        var graph = await ReadPublishedGraphAsync(db, building, cancellationToken);
        if (graph is null)
            return Results.NotFound();

        var branding = await db.TenantBrandings.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.TenantId == tenant.Id, cancellationToken);

        var floors = await db.Floors.AsNoTracking()
            .Where(floor => floor.BuildingId == building.Id && floor.Status == RecordStatus.Active && floor.IsPublic)
            .OrderBy(floor => floor.LevelIndex)
            .Select(floor => new FloorResponse(floor.Id, floor.Code, floor.Name, floor.LevelIndex))
            .ToListAsync(cancellationToken);

        var graphNodeIds = graph.Nodes.Select(node => node.Id).ToHashSet();
        var locations = await db.Locations.AsNoTracking()
            .Where(location =>
                location.BuildingId == building.Id &&
                location.Status == RecordStatus.Active &&
                location.IsPublic &&
                location.IsSearchable)
            .Select(location => new LocationResponse(
                location.Id,
                location.Name,
                location.ShortCode,
                location.Description,
                location.Keywords,
                location.Category!.Name,
                location.Category.Icon,
                location.FloorId,
                location.Floor!.Name,
                location.Id,
                0m,
                0m))
            .ToListAsync(cancellationToken);

        var locationsWithAccessNodes = locations
            .Select(location => (Location: location, Node: graph.Nodes.FirstOrDefault(candidate => candidate.LocationId == location.Id)))
            .Where(item => item.Node is not null)
            .Select(item => item.Location with
            {
                NodeId = item.Node!.Id,
                X = item.Node.X,
                Y = item.Node.Y,
            })
            .ToArray();

        var startPoints = await db.QrCodes.AsNoTracking()
            .Where(qr => qr.BuildingId == building.Id && qr.Status == QrStatus.Active &&
                         (qr.ExpiresAt == null || qr.ExpiresAt > DateTime.UtcNow))
            .Select(qr => new { qr.Label, qr.NodeId })
            .ToListAsync(cancellationToken);

        var starts = startPoints
            .Where(qr => graphNodeIds.Contains(qr.NodeId))
            .Select(qr =>
            {
                var node = graph.Nodes.Single(candidate => candidate.Id == qr.NodeId);
                var floor = floors.FirstOrDefault(candidate => candidate.Id == node.FloorId);
                return floor is null ? null : new StartPointResponse(qr.NodeId, qr.Label, floor.Id, floor.Name);
            })
            .Where(point => point is not null)
            .Select(point => point!)
            .ToArray();

        return Results.Ok(new BuildingResponse(
            tenant.Slug,
            branding?.DisplayName ?? tenant.Name,
            branding?.WelcomeMessage,
            building.Name,
            floors,
            locationsWithAccessNodes,
            starts,
            graph.Nodes.Select(node => new MapNodeResponse(
                node.Id, node.FloorId, node.Code, node.NodeType.ToString(), node.X, node.Y,
                node.LocationId, node.ConnectorName, node.IsAccessible)).ToArray(),
            graph.Edges.Select(edge => new MapEdgeResponse(
                edge.FromNodeId, edge.ToNodeId, edge.EdgeType.ToString(), edge.DistanceMeters,
                edge.IsBidirectional)).ToArray()));
    }

    private static async Task<IResult> ResolveQrAsync(
        string token,
        SmartNavDbContext db,
        ITenantContextSetter tenantSetter,
        CancellationToken cancellationToken)
    {
        if (token.Length != 22 || token.Any(character => !char.IsAsciiLetterOrDigit(character)))
            return Results.NotFound();

        var now = DateTime.UtcNow;
        var qr = await db.QrCodes.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(
                candidate => candidate.Token == token &&
                             candidate.Status == QrStatus.Active &&
                             (candidate.ExpiresAt == null || candidate.ExpiresAt > now),
                cancellationToken);
        if (qr is null)
            return Results.NotFound();

        var tenant = await db.Tenants.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(candidate =>
                candidate.Id == qr.TenantId &&
                candidate.Status == TenantStatus.Active &&
                candidate.DeletedAt == null, cancellationToken);
        if (tenant is null)
            return Results.NotFound();

        tenantSetter.SetTenant(tenant.Id);
        var building = await FindPublishedBuildingAsync(db, cancellationToken);
        if (building is null || building.Id != qr.BuildingId)
            return Results.NotFound();

        var graph = await ReadPublishedGraphAsync(db, building, cancellationToken);
        var node = graph?.Nodes.SingleOrDefault(candidate => candidate.Id == qr.NodeId);
        if (node is null)
            return Results.NotFound();

        var floor = await db.Floors.AsNoTracking()
            .Where(candidate => candidate.Id == node.FloorId && candidate.IsPublic)
            .Select(candidate => new FloorResponse(candidate.Id, candidate.Code, candidate.Name, candidate.LevelIndex))
            .FirstOrDefaultAsync(cancellationToken);
        if (floor is null)
            return Results.NotFound();

        var locationName = node.LocationId is { } locationId
            ? await db.Locations.AsNoTracking()
                .Where(candidate => candidate.Id == locationId && candidate.IsPublic)
                .Select(candidate => candidate.Name)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        var trackedQr = await db.QrCodes.FirstAsync(candidate => candidate.Id == qr.Id, cancellationToken);
        trackedQr.LastScannedAt = now;
        db.QrScanEvents.Add(new QrScanEvent { QrCodeId = qr.Id, ScannedAt = now });
        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(new ResolvedQrResponse(
            tenant.Slug,
            node.Id,
            locationName ?? node.ConnectorName ?? qr.Label,
            floor));
    }

    private static async Task<IResult> FindRouteAsync(
        string tenantSlug,
        FindRouteRequest request,
        SmartNavDbContext db,
        ITenantContextSetter tenantSetter,
        CancellationToken cancellationToken)
    {
        var tenant = await FindActiveTenantAsync(db, tenantSlug, cancellationToken);
        if (tenant is null)
            return Results.NotFound();

        tenantSetter.SetTenant(tenant.Id);
        var building = await FindPublishedBuildingAsync(db, cancellationToken);
        if (building is null)
            return Results.NotFound();

        if (!await db.QrCodes.AnyAsync(qr =>
                qr.BuildingId == building.Id &&
                qr.NodeId == request.StartNodeId &&
                qr.Status == QrStatus.Active &&
                (qr.ExpiresAt == null || qr.ExpiresAt > DateTime.UtcNow),
                cancellationToken))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["startNodeId"] = ["Choose a current location from an active QR checkpoint."],
            });

        var destination = await db.Locations.AsNoTracking()
            .Where(location =>
                location.Id == request.DestinationLocationId &&
                location.BuildingId == building.Id &&
                location.Status == RecordStatus.Active &&
                location.IsPublic &&
                location.IsSearchable)
            .Select(location => new
            {
                location.Id,
                location.Name,
                location.FloorId,
                FloorName = location.Floor!.Name,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (destination is null)
            return Results.NotFound();

        var graph = await ReadPublishedGraphAsync(db, building, cancellationToken);
        if (graph is null)
            return Results.NotFound();

        var destinationNode = graph.Nodes.SingleOrDefault(node => node.LocationId == destination.Id);
        if (destinationNode is null)
            return Results.Problem("The published navigation map is missing this destination node.");

        var allFloors = await db.Floors.AsNoTracking()
            .Where(floor => floor.BuildingId == building.Id)
            .Select(floor => new { floor.Id, floor.Name, floor.LevelIndex })
            .ToDictionaryAsync(floor => floor.Id, cancellationToken);

        var settings = await db.BuildingSettings.AsNoTracking()
            .SingleAsync(setting => setting.BuildingId == building.Id, cancellationToken);

        var nodesById = graph.Nodes.ToDictionary(node => node.Id);
        var locationsById = await db.Locations.AsNoTracking()
            .Where(location => location.BuildingId == building.Id && location.IsPublic)
            .ToDictionaryAsync(location => location.Id, location => location.Name, cancellationToken);
        var edges = graph.Edges.Select(edge =>
        {
            var from = nodesById[edge.FromNodeId];
            var to = nodesById[edge.ToNodeId];
            var floorDifference = Math.Abs(allFloors[to.FloorId].LevelIndex - allFloors[from.FloorId].LevelIndex);
            var connectorWait = from.ConnectorWaitSeconds ?? settings.DefaultLiftWaitSeconds;
            var connectorFloorTime = from.ConnectorSecondsPerFloor ?? settings.StairSecondsPerFloor;
            var seconds = edge.CostOverrideSeconds ?? edge.EdgeType switch
            {
                EdgeType.Lift => connectorWait + connectorFloorTime * floorDifference,
                EdgeType.Stair => Math.Max(
                    (double)edge.DistanceMeters / 0.7,
                    settings.StairSecondsPerFloor * floorDifference),
                _ => (double)edge.DistanceMeters / (double)settings.WalkingSpeedMps,
            };
            return new RouteEdge(
                edge.FromNodeId,
                edge.ToNodeId,
                edge.DistanceMeters,
                seconds,
                edge.IsAccessible,
                edge.IsRestricted,
                edge.RequiresStairs,
                edge.IsBidirectional);
        });

        var route = DijkstraRouter.FindShortestPath(
            graph.Nodes.Select(node => new RouteNode(node.Id, node.IsAccessible)),
            edges,
            request.StartNodeId,
            destinationNode.Id,
            request.AccessibleOnly);
        if (route is null)
            return Results.Conflict(new { message = "No route is available with the selected route preferences." });

        var routeEdges = graph.Edges.ToDictionary(edge => (edge.FromNodeId, edge.ToNodeId));
        var stepResponses = new List<RouteStepResponse>();
        var legResponses = new List<RouteLegResponse>();
        for (var index = 0; index < route.NodeIds.Count - 1; index++)
        {
            var from = nodesById[route.NodeIds[index]];
            var to = nodesById[route.NodeIds[index + 1]];
            var edge = routeEdges.TryGetValue((from.Id, to.Id), out var forward)
                ? forward
                : routeEdges[(to.Id, from.Id)] with
                {
                    FromNodeId = to.Id,
                    ToNodeId = from.Id,
                };
            var crossFloor = from.FloorId != to.FloorId;
            var targetName = to.LocationId is { } locationId && locationsById.TryGetValue(locationId, out var locationName)
                ? locationName
                : to.ConnectorName ?? (to.NodeType == NodeType.Junction ? "the next corridor junction" : "the next waypoint");
            var instruction = edge.EdgeType switch
            {
                EdgeType.Lift => $"Take {from.ConnectorName ?? to.ConnectorName ?? "the lift"} from {allFloors[from.FloorId].Name} to {allFloors[to.FloorId].Name}.",
                EdgeType.Stair => $"Take {from.ConnectorName ?? to.ConnectorName ?? "the stairs"} to {allFloors[to.FloorId].Name}.",
                _ => $"Follow the corridor for {Math.Round(edge.DistanceMeters)} m towards {targetName}.",
            };
            stepResponses.Add(new RouteStepResponse(
                index + 1,
                instruction,
                edge.DistanceMeters,
                from.FloorId,
                allFloors[from.FloorId].Name,
                edge.EdgeType.ToString(),
                to.Id == destinationNode.Id));
            legResponses.Add(new RouteLegResponse(
                from.Id, to.Id, from.FloorId, to.FloorId, from.X, from.Y, to.X, to.Y,
                edge.DistanceMeters, edge.EdgeType.ToString()));
        }

        var finalNode = nodesById[destinationNode.Id];
        stepResponses.Add(new RouteStepResponse(
            stepResponses.Count + 1,
            $"You have arrived at {destination.Name}.",
            0,
            finalNode.FloorId,
            destination.FloorName,
            "Destination",
            true));

        return Results.Ok(new RouteResponse(
            destination.Name,
            destination.FloorName,
            route.DistanceMeters,
            route.TravelSeconds,
            request.AccessibleOnly,
            route.NodeIds,
            legResponses,
            stepResponses));
    }

    private static Task<Tenant?> FindActiveTenantAsync(
        SmartNavDbContext db,
        string tenantSlug,
        CancellationToken cancellationToken) =>
        db.Tenants.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(tenant =>
                tenant.Slug == tenantSlug &&
                tenant.Status == TenantStatus.Active &&
                tenant.DeletedAt == null,
                cancellationToken);

    private static Task<Building?> FindPublishedBuildingAsync(
        SmartNavDbContext db,
        CancellationToken cancellationToken) =>
        db.Buildings.AsNoTracking()
            .FirstOrDefaultAsync(building => building.Status == BuildingStatus.Published, cancellationToken);

    private static async Task<PublishedGraph?> ReadPublishedGraphAsync(
        SmartNavDbContext db,
        Building building,
        CancellationToken cancellationToken)
    {
        if (building.PublishedVersion is not { } version)
            return null;

        var json = await db.GraphSnapshots.AsNoTracking()
            .Where(snapshot => snapshot.BuildingId == building.Id && snapshot.Version == version)
            .Select(snapshot => snapshot.SnapshotJson)
            .FirstOrDefaultAsync(cancellationToken);
        return json is null ? null : JsonSerializer.Deserialize<PublishedGraph>(json);
    }

    public sealed record BuildingResponse(
        string TenantSlug,
        string Name,
        string? WelcomeMessage,
        string BuildingName,
        IReadOnlyList<FloorResponse> Floors,
        IReadOnlyList<LocationResponse> Locations,
        IReadOnlyList<StartPointResponse> StartPoints,
        IReadOnlyList<MapNodeResponse> Nodes,
        IReadOnlyList<MapEdgeResponse> Edges);

    public sealed record FloorResponse(Guid Id, string Code, string Name, int LevelIndex);
    public sealed record LocationResponse(
        Guid Id,
        string Name,
        string? Code,
        string? Description,
        string? Keywords,
        string Category,
        string? Icon,
        Guid FloorId,
        string FloorName,
        Guid NodeId,
        decimal X = 0,
        decimal Y = 0);
    public sealed record StartPointResponse(Guid NodeId, string Label, Guid FloorId, string FloorName);
    public sealed record MapNodeResponse(
        Guid Id,
        Guid FloorId,
        string Code,
        string Type,
        decimal X,
        decimal Y,
        Guid? LocationId,
        string? ConnectorName,
        bool IsAccessible);
    public sealed record MapEdgeResponse(
        Guid FromNodeId,
        Guid ToNodeId,
        string Type,
        decimal DistanceMeters,
        bool IsBidirectional);
    public sealed record ResolvedQrResponse(string TenantSlug, Guid NodeId, string Label, FloorResponse Floor);
    public sealed record FindRouteRequest(Guid StartNodeId, Guid DestinationLocationId, bool AccessibleOnly);
    public sealed record RouteResponse(
        string Destination,
        string Floor,
        decimal DistanceMeters,
        double TravelSeconds,
        bool Accessible,
        IReadOnlyList<Guid> NodeIds,
        IReadOnlyList<RouteLegResponse> Legs,
        IReadOnlyList<RouteStepResponse> Steps);
    public sealed record RouteLegResponse(
        Guid FromNodeId,
        Guid ToNodeId,
        Guid FromFloorId,
        Guid ToFloorId,
        decimal FromX,
        decimal FromY,
        decimal ToX,
        decimal ToY,
        decimal DistanceMeters,
        string Type);
    public sealed record RouteStepResponse(
        int Number,
        string Instruction,
        decimal DistanceMeters,
        Guid FloorId,
        string FloorName,
        string Type,
        bool IsDestination);
}
