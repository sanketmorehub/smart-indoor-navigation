using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartNav.Application.Abstractions;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Common;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Navigation;
using SmartNav.Domain.Qr;
using SmartNav.Domain.Tenancy;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Navigation;

namespace SmartNav.Infrastructure.Seeding;

/// <summary>
/// Seeds platform reference data + the "Aura Hospital" demo tenant.
/// This is DATA ONLY - nothing in the application code knows about hospitals.
/// Idempotently upgrades the demo tenant and publishes its illustrative hospital floor plan.
/// </summary>
public sealed class DemoDataSeeder(
    SmartNavDbContext db,
    ITenantContextSetter tenantSetter,
    IQrTokenGenerator tokens,
    IConfiguration config,
    ILogger<DemoDataSeeder> log)
{
    public const string DemoTenantSlug = "abc-hospital";

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedPlatformAsync(ct);

        // Global filters hide tenant rows until a tenant is set, so check existence unfiltered.
        var tenant = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Slug == DemoTenantSlug, ct);
        if (tenant is not null)
        {
            tenantSetter.SetTenant(tenant.Id);
            tenant.Name = "Aura Hospital";
            var branding = await db.TenantBrandings.FirstOrDefaultAsync(ct);
            if (branding is null)
            {
                db.TenantBrandings.Add(new TenantBranding
                {
                    DisplayName = "Aura Hospital",
                    PrimaryColor = "#0E7C86",
                    SecondaryColor = "#F2A541",
                    WelcomeMessage = "Welcome to Aura Hospital. Scan any QR code to find your way.",
                });
            }
            else
            {
                branding.DisplayName = "Aura Hospital";
                branding.WelcomeMessage = "Welcome to Aura Hospital. Scan any QR code to find your way.";
            }

            var building = await db.Buildings.FirstOrDefaultAsync(b => b.Slug == "main", ct)
                ?? throw new InvalidOperationException("The Aura Hospital demo building is missing.");
            building.Name = "Aura Hospital";
            await EnsureDemoHospitalContentAsync(building, ct);
            await db.SaveChangesAsync(ct);
            await PublishDemoGraphAsync(ct);
            await LogDemoQrCodesAsync(ct);
            return;
        }

        tenant = new Tenant { Slug = DemoTenantSlug, Name = "Aura Hospital", OrganizationType = "HOSPITAL", TimeZone = "Asia/Kolkata" };
        db.Tenants.Add(tenant);
        tenantSetter.SetTenant(tenant.Id); // every tenant-owned row below is stamped by the interceptor
        await SeedDemoTenantAsync(ct);
        var demoBuilding = await db.Buildings.SingleAsync(building => building.Slug == "main", ct);
        await EnsureDemoHospitalContentAsync(demoBuilding, ct);
        await db.SaveChangesAsync(ct);
        await PublishDemoGraphAsync(ct);
        await LogDemoQrCodesAsync(ct);
    }

    private async Task SeedPlatformAsync(CancellationToken ct)
    {
        if (!await db.Permissions.AnyAsync(ct))
        {
            var codes = PermissionCodes.SystemRoles.Values.SelectMany(x => x).Distinct()
                .Select(c => new Permission { Code = c, Description = c, Scope = PermissionScope.Tenant })
                .Append(new Permission { Code = PermissionCodes.PlatformTenantsManage, Description = "Manage tenants", Scope = PermissionScope.Platform })
                .Append(new Permission { Code = PermissionCodes.PlatformAnalyticsRead, Description = "Platform analytics", Scope = PermissionScope.Platform })
                .ToList();
            db.Permissions.AddRange(codes);

            foreach (var (code, perms) in PermissionCodes.SystemRoles)
            {
                var role = new Role { Code = code, Name = code[..1] + code[1..].ToLowerInvariant(), IsSystem = true };
                role.Permissions.AddRange(codes.Where(p => perms.Contains(p.Code))
                    .Select(p => new RolePermission { RoleId = role.Id, PermissionId = p.Id }));
                db.Roles.Add(role);
            }
        }

        if (!await db.LocationCategories.AnyAsync(c => c.TenantId == null, ct))
            db.LocationCategories.AddRange(ReferenceData.GlobalCategories());

        await db.SaveChangesAsync(ct);
    }

    private async Task SeedDemoTenantAsync(CancellationToken ct)
    {
        db.TenantBrandings.Add(new TenantBranding
        {
            DisplayName = "Aura Hospital", PrimaryColor = "#0E7C86", SecondaryColor = "#F2A541",
            WelcomeMessage = "Welcome to Aura Hospital. Scan any QR code to find your way.",
        });
        db.TenantSettings.Add(new TenantSettings());

        var building = new Building { Slug = "main", Name = "Aura Hospital", City = "Pune", Country = "IN", TimeZone = "Asia/Kolkata" };
        db.Buildings.Add(building);
        db.BuildingSettings.Add(new BuildingSettings { BuildingId = building.Id });

        var ground = new Floor { BuildingId = building.Id, LevelIndex = 0, Code = "G", Name = "Ground Floor · Emergency & Patient Access" };
        var first = new Floor { BuildingId = building.Id, LevelIndex = 1, Code = "1", Name = "First Floor · Specialist OPD & Rehabilitation", ElevationMeters = 4.5m };
        db.Floors.AddRange(ground, first);

        var cat = await db.LocationCategories.Where(c => c.TenantId == null).ToDictionaryAsync(c => c.Code, ct);
        Location Loc(Floor f, string name, string category, string? keywords = null) =>
            new() { BuildingId = building.Id, FloorId = f.Id, Name = name, CategoryId = cat[category].Id, Keywords = keywords };

        var reception = Loc(ground, "Reception", "RECEPTION", "front desk, enquiry, help desk");
        var emergency = Loc(ground, "Emergency", "EMERGENCY", "casualty, ER, trauma");
        var medical = Loc(ground, "Medical Store", "MEDICAL_STORE", "pharmacy, chemist, medicines");
        var cardiology = Loc(first, "Cardiology", "DEPARTMENT", "heart, cardiac");
        var neurology = Loc(first, "Neurology", "DEPARTMENT", "brain, nerves");
        db.Locations.AddRange(reception, emergency, medical, cardiology, neurology);

        var liftA = new VerticalConnector
        {
            BuildingId = building.Id, Code = "LIFT-A", Name = "Lift A", ConnectorType = ConnectorType.Lift,
            IsAccessible = true, WaitSeconds = 45, SecondsPerFloor = 8,
        };
        var stair1 = new VerticalConnector
        {
            BuildingId = building.Id, Code = "STAIR-1", Name = "Stairs 1", ConnectorType = ConnectorType.Stair,
        };
        db.VerticalConnectors.AddRange(liftA, stair1);

        // Coordinates in viewBox units of a 1000x700 floor SVG; scale 0.05 m/unit (200 units = 10 m).
        NavigationNode Node(Floor f, string code, NodeType t, decimal x, decimal y, Location? l = null, VerticalConnector? c = null) =>
            new() { BuildingId = building.Id, FloorId = f.Id, Code = code, NodeType = t, X = x, Y = y,
                    LocationId = l?.Id, ConnectorId = c?.Id, IsAccessible = c?.IsAccessible ?? true };

        var g = new Dictionary<string, NavigationNode>
        {
            ["G-ENT"] = Node(ground, "G-ENT", NodeType.Entrance, 100, 500),
            ["G-REC"] = Node(ground, "G-REC", NodeType.LocationAccess, 220, 500, reception),
            ["G-J1"] = Node(ground, "G-J1", NodeType.Junction, 400, 500),
            ["G-EMG"] = Node(ground, "G-EMG", NodeType.LocationAccess, 400, 640, emergency),
            ["G-MED"] = Node(ground, "G-MED", NodeType.LocationAccess, 650, 500, medical),
            ["G-LIFTA"] = Node(ground, "G-LIFTA", NodeType.Connector, 400, 300, c: liftA),
            ["G-J2"] = Node(ground, "G-J2", NodeType.Junction, 800, 500),
            ["G-ST1"] = Node(ground, "G-ST1", NodeType.Connector, 800, 300, c: stair1),
            ["1-LIFTA"] = Node(first, "1-LIFTA", NodeType.Connector, 400, 300, c: liftA),
            ["1-J1"] = Node(first, "1-J1", NodeType.Junction, 400, 500),
            ["1-CAR"] = Node(first, "1-CAR", NodeType.LocationAccess, 220, 500, cardiology),
            ["1-NEU"] = Node(first, "1-NEU", NodeType.LocationAccess, 650, 500, neurology),
            ["1-J2"] = Node(first, "1-J2", NodeType.Junction, 800, 500),
            ["1-ST1"] = Node(first, "1-ST1", NodeType.Connector, 800, 300, c: stair1),
        };
        db.NavigationNodes.AddRange(g.Values);

        const decimal metersPerUnit = 0.05m;
        NavigationEdge Walk(string a, string b)
        {
            var (na, nb) = (g[a], g[b]);
            var dx = (double)(na.X - nb.X); var dy = (double)(na.Y - nb.Y);
            var meters = Math.Round((decimal)Math.Sqrt(dx * dx + dy * dy) * metersPerUnit, 2);
            return new() { BuildingId = building.Id, FromNodeId = na.Id, ToNodeId = nb.Id, DistanceMeters = meters };
        }

        db.NavigationEdges.AddRange(
            Walk("G-ENT", "G-REC"), Walk("G-REC", "G-J1"), Walk("G-J1", "G-EMG"), Walk("G-J1", "G-LIFTA"),
            Walk("G-J1", "G-MED"), Walk("G-MED", "G-J2"), Walk("G-J2", "G-ST1"),
            Walk("1-LIFTA", "1-J1"), Walk("1-J1", "1-CAR"), Walk("1-J1", "1-NEU"), Walk("1-NEU", "1-J2"), Walk("1-J2", "1-ST1"),
            new NavigationEdge { BuildingId = building.Id, FromNodeId = g["G-LIFTA"].Id, ToNodeId = g["1-LIFTA"].Id,
                                 EdgeType = EdgeType.Lift, DistanceMeters = 4.5m },
            new NavigationEdge { BuildingId = building.Id, FromNodeId = g["G-ST1"].Id, ToNodeId = g["1-ST1"].Id,
                                 EdgeType = EdgeType.Stair, DistanceMeters = 9m, IsAccessible = false, RequiresStairs = true });

        QrCode Qr(string node, string label, QrPurpose purpose, Location? l = null) =>
            new() { BuildingId = building.Id, NodeId = g[node].Id, LocationId = l?.Id, Token = tokens.NewToken(), Label = label, Purpose = purpose };

        var qrs = new[]
        {
            Qr("G-REC", "Reception", QrPurpose.Start, reception),
            Qr("1-CAR", "Cardiology", QrPurpose.Start, cardiology),
            Qr("G-LIFTA", "Lift A - Ground Floor", QrPurpose.Transition),
            Qr("1-LIFTA", "Lift A - First Floor", QrPurpose.Transition),
        };
        db.QrCodes.AddRange(qrs);

        await SeedDemoOwnerAsync(ct);
        await db.SaveChangesAsync(ct);
    }

    private async Task EnsureDemoHospitalContentAsync(Building building, CancellationToken ct)
    {
        var floorsByLevel = await db.Floors
            .Where(floor => floor.BuildingId == building.Id)
            .ToDictionaryAsync(floor => floor.LevelIndex, ct);
        foreach (var definition in DemoHospitalFloorPlan.Floors)
        {
            if (!floorsByLevel.TryGetValue(definition.LevelIndex, out var floor))
            {
                floor = new Floor { BuildingId = building.Id, LevelIndex = definition.LevelIndex, Code = definition.Code, Name = definition.Name };
                db.Floors.Add(floor);
                floorsByLevel.Add(definition.LevelIndex, floor);
            }

            floor.Code = definition.Code;
            floor.Name = definition.Name;
            floor.ElevationMeters = definition.LevelIndex * 4.5m;
            floor.IsPublic = true;
            floor.Status = RecordStatus.Active;
        }

        var floorsByCode = floorsByLevel.Values.ToDictionary(floor => floor.Code, StringComparer.OrdinalIgnoreCase);
        var categories = await db.LocationCategories
            .Where(category => category.TenantId == null && category.IsActive)
            .ToDictionaryAsync(category => category.Code, ct);
        var locations = await db.Locations
            .Where(location => location.BuildingId == building.Id)
            .ToListAsync(ct);
        var locationsByName = locations.ToDictionary(location => location.Name, StringComparer.OrdinalIgnoreCase);
        if (locationsByName.TryGetValue("Neurology", out var neurology) &&
            !locationsByName.ContainsKey("Neurology & Neurosurgery"))
        {
            locationsByName.Remove("Neurology");
            neurology.Name = "Neurology & Neurosurgery";
            locationsByName.Add(neurology.Name, neurology);
        }

        foreach (var (name, floorCode, description, keywords) in new[]
        {
            ("Reception", "G", "Main visitor reception and enquiry desk.", "front desk enquiry help desk"),
            ("Emergency", "G", "Emergency and trauma-care arrival point. In an emergency, contact hospital staff.", "casualty ER trauma 24/7"),
            ("Medical Store", "G", "Medical store and pharmacy information.", "pharmacy chemist medicines"),
            ("Cardiology", "3", "Heart-disease management, heart-failure clinics, and pacemaker monitoring.", "heart cardiac heart failure pacemaker"),
        })
        {
            if (locationsByName.TryGetValue(name, out var location))
            {
                location.FloorId = floorsByCode[floorCode].Id;
                location.Description = description;
                location.Keywords = keywords;
                location.IsPublic = true;
                location.IsSearchable = true;
                location.Status = RecordStatus.Active;
            }
        }

        foreach (var service in DemoHospitalFloorPlan.Services)
        {
            if (!categories.TryGetValue(service.CategoryCode, out var category))
                throw new InvalidOperationException($"The demo service category '{service.CategoryCode}' is missing.");

            if (!locationsByName.TryGetValue(service.Name, out var location))
            {
                location = new Location
                {
                    BuildingId = building.Id,
                    FloorId = floorsByCode[service.FloorCode].Id,
                    Name = service.Name,
                    CategoryId = category.Id,
                };
                locations.Add(location);
                locationsByName.Add(service.Name, location);
                db.Locations.Add(location);
            }

            location.FloorId = floorsByCode[service.FloorCode].Id;
            location.CategoryId = category.Id;
            location.Description = service.Description;
            location.Keywords = service.Keywords;
            location.IsPublic = true;
            location.IsSearchable = true;
            location.Status = RecordStatus.Active;
        }

        var lift = await db.VerticalConnectors.FirstOrDefaultAsync(
            connector => connector.BuildingId == building.Id && connector.Code == "LIFT-A", ct);
        if (lift is null)
        {
            lift = new VerticalConnector
            {
                BuildingId = building.Id, Code = "LIFT-A", Name = "Lift A", ConnectorType = ConnectorType.Lift,
                IsAccessible = true, WaitSeconds = 45, SecondsPerFloor = 8,
            };
            db.VerticalConnectors.Add(lift);
        }

        var stairs = await db.VerticalConnectors.FirstOrDefaultAsync(
            connector => connector.BuildingId == building.Id && connector.Code == "STAIR-1", ct);
        if (stairs is null)
        {
            stairs = new VerticalConnector
            {
                BuildingId = building.Id, Code = "STAIR-1", Name = "Stairs 1", ConnectorType = ConnectorType.Stair,
                IsAccessible = false, SecondsPerFloor = 15,
            };
            db.VerticalConnectors.Add(stairs);
        }

        var nodes = await db.NavigationNodes.Where(node => node.BuildingId == building.Id).ToListAsync(ct);
        var nodesByLocation = nodes.Where(node => node.LocationId is not null)
            .GroupBy(node => node.LocationId!.Value)
            .ToDictionary(group => group.Key, group => group.First());
        var junctions = new Dictionary<Guid, NavigationNode>();
        var liftNodes = new Dictionary<Guid, NavigationNode>();
        var stairNodes = new Dictionary<Guid, NavigationNode>();
        foreach (var floor in floorsByLevel.Values)
        {
            var junction = nodes.FirstOrDefault(node => node.Code == $"{floor.Code}-J1");
            if (junction is null)
            {
                junction = NewNode(building, floor, $"{floor.Code}-J1", NodeType.Junction, 500, 350);
                nodes.Add(junction);
                db.NavigationNodes.Add(junction);
            }
            junction.FloorId = floor.Id;
            junction.NodeType = NodeType.Junction;
            junction.IsActive = true;
            junction.IsAccessible = true;
            junction.X = 500;
            junction.Y = 350;
            junctions.Add(floor.Id, junction);

            var liftNode = nodes.FirstOrDefault(node => node.FloorId == floor.Id && node.ConnectorId == lift.Id);
            if (liftNode is null)
            {
                liftNode = NewNode(building, floor, $"{floor.Code}-LIFT-A", NodeType.Connector, 430, 350, connectorId: lift.Id);
                nodes.Add(liftNode);
                db.NavigationNodes.Add(liftNode);
            }
            SetConnectorNode(liftNode, floor, lift, 430, 350);
            liftNodes.Add(floor.Id, liftNode);

            var stairNode = nodes.FirstOrDefault(node => node.FloorId == floor.Id && node.ConnectorId == stairs.Id);
            if (stairNode is null)
            {
                stairNode = NewNode(building, floor, $"{floor.Code}-STAIR-1", NodeType.Connector, 570, 350, connectorId: stairs.Id);
                nodes.Add(stairNode);
                db.NavigationNodes.Add(stairNode);
            }
            SetConnectorNode(stairNode, floor, stairs, 570, 350);
            stairNodes.Add(floor.Id, stairNode);
        }

        var locationsByFloor = locations
            .OrderBy(location => location.Name, StringComparer.OrdinalIgnoreCase)
            .GroupBy(location => location.FloorId)
            .ToDictionary(group => group.Key, group => group.ToArray());
        foreach (var floor in floorsByLevel.Values)
        {
            var floorLocations = locationsByFloor.GetValueOrDefault(floor.Id, []);
            for (var index = 0; index < floorLocations.Length; index++)
            {
                var location = floorLocations[index];
                if (!nodesByLocation.TryGetValue(location.Id, out var node))
                {
                    node = NewNode(
                        building,
                        floor,
                        $"L-{floor.Code}-{location.Id.ToString("N")[^12..]}",
                        NodeType.LocationAccess,
                        0,
                        0,
                        location.Id);
                    nodes.Add(node);
                    nodesByLocation.Add(location.Id, node);
                    db.NavigationNodes.Add(node);
                }

                var column = index % 5;
                var row = index / 5;
                node.FloorId = floor.Id;
                node.NodeType = NodeType.LocationAccess;
                node.X = 110 + column * 195;
                node.Y = 120 + row * 150;
                node.IsAccessible = true;
                node.IsActive = true;
            }
        }

        var entrance = nodes.FirstOrDefault(node => node.Code == "G-ENT");
        if (entrance is not null)
        {
            entrance.FloorId = floorsByCode["G"].Id;
            entrance.NodeType = NodeType.Entrance;
            entrance.X = 90;
            entrance.Y = 350;
            entrance.IsAccessible = true;
            entrance.IsActive = true;
        }

        var managedNodeIds = junctions.Values.Concat(liftNodes.Values).Concat(stairNodes.Values)
            .Concat(nodesByLocation.Values).Select(node => node.Id).ToHashSet();
        foreach (var node in nodes.Where(node =>
                     node.LocationId is null && node.ConnectorId is null &&
                     !managedNodeIds.Contains(node.Id) && node.Code != "G-ENT"))
            node.IsActive = false;

        var duplicateNodeCodes = nodes.GroupBy(node => node.Code, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        if (duplicateNodeCodes.Length > 0)
            throw new InvalidOperationException($"Duplicate demo navigation node codes: {string.Join(", ", duplicateNodeCodes)}.");

        var activeQrNodes = await db.QrCodes
            .Where(qr => qr.BuildingId == building.Id && qr.Status == QrStatus.Active)
            .Select(qr => qr.NodeId)
            .ToListAsync(ct);
        foreach (var floor in floorsByLevel.Values)
        {
            var liftNode = liftNodes[floor.Id];
            if (activeQrNodes.Contains(liftNode.Id))
                continue;

            db.QrCodes.Add(new QrCode
            {
                BuildingId = building.Id,
                NodeId = liftNode.Id,
                Token = tokens.NewToken(),
                Label = $"Lift A · {floor.Name}",
                Purpose = QrPurpose.Start,
            });
        }

        var oldEdges = await db.NavigationEdges.Where(edge => edge.BuildingId == building.Id).ToListAsync(ct);
        db.NavigationEdges.RemoveRange(oldEdges);
        await db.SaveChangesAsync(ct);

        decimal Distance(NavigationNode from, NavigationNode to)
        {
            var dx = (double)(from.X - to.X);
            var dy = (double)(from.Y - to.Y);
            return Math.Max(0.5m, Math.Round((decimal)Math.Sqrt(dx * dx + dy * dy) * 0.05m, 2));
        }

        NavigationEdge Walk(NavigationNode from, NavigationNode to) =>
            new() { BuildingId = building.Id, FromNodeId = from.Id, ToNodeId = to.Id, DistanceMeters = Distance(from, to) };

        var edges = new List<NavigationEdge>();
        foreach (var floor in floorsByLevel.Values)
        {
            var junction = junctions[floor.Id];
            edges.Add(Walk(junction, liftNodes[floor.Id]));
            edges.Add(Walk(junction, stairNodes[floor.Id]));
            edges.AddRange(locationsByFloor.GetValueOrDefault(floor.Id, [])
                .Select(location => Walk(junction, nodesByLocation[location.Id])));
            if (entrance is not null && floor.Code == "G")
                edges.Add(Walk(junction, entrance));
        }

        var orderedFloors = DemoHospitalFloorPlan.Floors.OrderBy(floor => floor.LevelIndex).ToArray();
        for (var fromIndex = 0; fromIndex < orderedFloors.Length; fromIndex++)
        {
            for (var toIndex = fromIndex + 1; toIndex < orderedFloors.Length; toIndex++)
            {
                var fromFloor = floorsByCode[orderedFloors[fromIndex].Code];
                var toFloor = floorsByCode[orderedFloors[toIndex].Code];
                edges.Add(new NavigationEdge
                {
                    BuildingId = building.Id,
                    FromNodeId = liftNodes[fromFloor.Id].Id,
                    ToNodeId = liftNodes[toFloor.Id].Id,
                    EdgeType = EdgeType.Lift,
                    DistanceMeters = Math.Abs(fromFloor.LevelIndex - toFloor.LevelIndex) * 4.5m,
                    IsAccessible = true,
                });
            }
        }

        for (var index = 0; index < orderedFloors.Length - 1; index++)
        {
            var fromFloor = floorsByCode[orderedFloors[index].Code];
            var toFloor = floorsByCode[orderedFloors[index + 1].Code];
            edges.Add(new NavigationEdge
            {
                BuildingId = building.Id,
                FromNodeId = stairNodes[fromFloor.Id].Id,
                ToNodeId = stairNodes[toFloor.Id].Id,
                EdgeType = EdgeType.Stair,
                DistanceMeters = 9,
                IsAccessible = false,
                RequiresStairs = true,
            });
        }
        db.NavigationEdges.AddRange(edges);

        static NavigationNode NewNode(
            Building parent,
            Floor floor,
            string code,
            NodeType type,
            decimal x,
            decimal y,
            Guid? locationId = null,
            Guid? connectorId = null) =>
            new()
            {
                BuildingId = parent.Id,
                FloorId = floor.Id,
                Code = code,
                NodeType = type,
                X = x,
                Y = y,
                LocationId = locationId,
                ConnectorId = connectorId,
                IsAccessible = true,
            };

        static void SetConnectorNode(
            NavigationNode node,
            Floor floor,
            VerticalConnector connector,
            decimal x,
            decimal y)
        {
            node.FloorId = floor.Id;
            node.NodeType = NodeType.Connector;
            node.ConnectorId = connector.Id;
            node.X = x;
            node.Y = y;
            node.IsAccessible = connector.IsAccessible;
            node.IsActive = true;
        }
    }

    private async Task PublishDemoGraphAsync(CancellationToken ct)
    {
        var building = await db.Buildings.SingleOrDefaultAsync(building => building.Slug == "main", ct)
            ?? throw new InvalidOperationException("The Aura Hospital demo building is missing.");

        var connectors = await db.VerticalConnectors
            .Where(connector => connector.BuildingId == building.Id)
            .ToDictionaryAsync(connector => connector.Id, ct);
        var nodes = await db.NavigationNodes
            .Where(node => node.BuildingId == building.Id && node.IsActive)
            .OrderBy(node => node.Code)
            .ToListAsync(ct);
        var edges = await db.NavigationEdges
            .Where(edge => edge.BuildingId == building.Id && edge.IsActive)
            .OrderBy(edge => edge.FromNodeId)
            .ThenBy(edge => edge.ToNodeId)
            .ToListAsync(ct);
        if (nodes.Count == 0 || edges.Count == 0)
            throw new InvalidOperationException("The Aura Hospital demo navigation graph has no active nodes or edges.");

        var publishedGraph = new PublishedGraph(
            nodes.Select(node =>
            {
                var connector = node.ConnectorId is { } id ? connectors.GetValueOrDefault(id) : null;
                return new PublishedNode(
                    node.Id, node.FloorId, node.Code, node.NodeType, node.X, node.Y, node.LocationId,
                    node.ConnectorId, connector?.Name, connector?.ConnectorType, connector?.WaitSeconds,
                    connector?.SecondsPerFloor, node.IsAccessible);
            }).ToArray(),
            edges.Select(edge => new PublishedEdge(
                edge.FromNodeId, edge.ToNodeId, edge.EdgeType, edge.IsBidirectional, edge.DistanceMeters,
                edge.CostOverrideSeconds, edge.IsAccessible, edge.RequiresStairs, edge.IsRestricted)).ToArray());
        var snapshotJson = JsonSerializer.Serialize(publishedGraph);
        if (building.PublishedVersion is { } currentVersion)
        {
            var currentSnapshot = await db.GraphSnapshots
                .Where(snapshot => snapshot.BuildingId == building.Id && snapshot.Version == currentVersion)
                .Select(snapshot => snapshot.SnapshotJson)
                .FirstOrDefaultAsync(ct);
            if (currentSnapshot == snapshotJson)
                return;
        }

        var publishedAt = DateTime.UtcNow;
        var nextVersion = (building.PublishedVersion ?? 0) + 1;
        db.GraphSnapshots.Add(new GraphSnapshot
        {
            BuildingId = building.Id,
            Version = nextVersion,
            SnapshotJson = snapshotJson,
            NodeCount = publishedGraph.Nodes.Count,
            EdgeCount = publishedGraph.Edges.Count,
            PublishedAt = publishedAt,
        });
        building.PublishedVersion = nextVersion;
        building.PublishedAt = publishedAt;
        building.Status = BuildingStatus.Published;
        await db.SaveChangesAsync(ct);
        log.LogInformation(
            "Published Aura Hospital navigation graph version {Version} with {NodeCount} nodes and {EdgeCount} edges.",
            nextVersion,
            publishedGraph.Nodes.Count,
            publishedGraph.Edges.Count);
    }

    private async Task LogDemoQrCodesAsync(CancellationToken ct)
    {
        var codes = await db.QrCodes.AsNoTracking()
            .Where(qr => qr.Status == QrStatus.Active)
            .OrderBy(qr => qr.Label)
            .ToListAsync(ct);
        foreach (var code in codes)
            log.LogInformation("Demo QR {Label}: /q/{Token}", code.Label, code.Token);
    }

    /// <summary>Owner account only if a password is supplied via config/user-secrets. Never committed.</summary>
    private async Task SeedDemoOwnerAsync(CancellationToken ct)
    {
        var email = config["Seed:DemoOwnerEmail"] ?? "owner@abc-hospital.demo";
        var password = config["Seed:DemoOwnerPassword"];
        if (string.IsNullOrWhiteSpace(password))
        {
            log.LogWarning("Seed:DemoOwnerPassword not set - demo owner user NOT created.");
            return;
        }

        var normalized = email.Trim().ToUpperInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalized, ct);
        if (user is null)
        {
            user = new User { Email = email, NormalizedEmail = normalized, DisplayName = "Aura Hospital Owner", PasswordHash = "" };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            db.Users.Add(user);
        }

        var ownerRole = await db.Roles.SingleAsync(r => r.TenantId == null && r.Code == "OWNER", ct);
        db.TenantMemberships.Add(new TenantMembership { UserId = user.Id, RoleId = ownerRole.Id });
    }
}
