using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartNav.Application.Abstractions;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Common;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Navigation;
using SmartNav.Domain.Qr;
using SmartNav.Domain.Tenancy;
using SmartNav.Infrastructure.Persistence;

namespace SmartNav.Infrastructure.Seeding;

/// <summary>
/// Seeds platform reference data + the "ABC Hospital" demo tenant.
/// This is DATA ONLY - nothing in the application code knows about hospitals.
/// Idempotent: skips anything that already exists.
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
            log.LogInformation("Demo tenant '{Slug}' already exists - skipping.", DemoTenantSlug);
            return;
        }

        tenant = new Tenant { Slug = DemoTenantSlug, Name = "ABC Hospital", OrganizationType = "HOSPITAL", TimeZone = "Asia/Kolkata" };
        db.Tenants.Add(tenant);
        tenantSetter.SetTenant(tenant.Id); // every tenant-owned row below is stamped by the interceptor
        await SeedDemoTenantAsync(ct);
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
            DisplayName = "ABC Hospital", PrimaryColor = "#0E7C86", SecondaryColor = "#F2A541",
            WelcomeMessage = "Welcome to ABC Hospital. Scan any QR code to find your way.",
        });
        db.TenantSettings.Add(new TenantSettings());

        var building = new Building { Slug = "main", Name = "ABC Hospital Main Building", City = "Pune", Country = "IN", TimeZone = "Asia/Kolkata" };
        db.Buildings.Add(building);
        db.BuildingSettings.Add(new BuildingSettings { BuildingId = building.Id });

        var ground = new Floor { BuildingId = building.Id, LevelIndex = 0, Code = "G", Name = "Ground Floor" };
        var first = new Floor { BuildingId = building.Id, LevelIndex = 1, Code = "1", Name = "First Floor", ElevationMeters = 4.5m };
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

        // Building stays DRAFT; it is published in Phase 6 (graph snapshot).
        foreach (var q in qrs) log.LogInformation("Demo QR {Label}: /q/{Token}", q.Label, q.Token);
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
            user = new User { Email = email, NormalizedEmail = normalized, DisplayName = "ABC Hospital Owner", PasswordHash = "" };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            db.Users.Add(user);
        }

        var ownerRole = await db.Roles.SingleAsync(r => r.TenantId == null && r.Code == "OWNER", ct);
        db.TenantMemberships.Add(new TenantMembership { UserId = user.Id, RoleId = ownerRole.Id });
    }
}
