namespace SmartNav.Domain.Identity;

/// <summary>
/// Authorization checks PERMISSIONS, never role names, so tenants can get custom roles later.
/// </summary>
public static class PermissionCodes
{
    // Platform
    public const string PlatformTenantsManage = "platform.tenants.manage";
    public const string PlatformAnalyticsRead = "platform.analytics.read";

    // Tenant
    public const string OrgManage = "org.manage";
    public const string BrandingManage = "branding.manage";
    public const string UsersManage = "users.manage";
    public const string BuildingsManage = "buildings.manage";
    public const string FloorsManage = "floors.manage";
    public const string MapsManage = "maps.manage";
    public const string LocationsManage = "locations.manage";
    public const string GraphEdit = "graph.edit";
    public const string GraphPublish = "graph.publish";
    public const string QrManage = "qr.manage";
    public const string QrPrint = "qr.print";
    public const string ClosuresManage = "closures.manage";
    public const string AuditRead = "audit.read";
    public const string ContentRead = "content.read";

    public static readonly IReadOnlyDictionary<string, string[]> SystemRoles = new Dictionary<string, string[]>
    {
        ["OWNER"] = [OrgManage, BrandingManage, UsersManage, BuildingsManage, FloorsManage, MapsManage,
                     LocationsManage, GraphEdit, GraphPublish, QrManage, QrPrint, ClosuresManage, AuditRead, ContentRead],
        ["ADMIN"] = [FloorsManage, MapsManage, LocationsManage, GraphEdit, GraphPublish, QrManage, QrPrint,
                     ClosuresManage, AuditRead, ContentRead],
        ["STAFF"] = [ContentRead, QrPrint],
    };
}
