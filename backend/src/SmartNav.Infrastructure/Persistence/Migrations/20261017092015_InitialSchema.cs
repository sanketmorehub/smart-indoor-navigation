using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNav.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    ActorUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    ActorType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    ChangesJson = table.Column<string>(type: "json", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CorrelationId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Scope = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Slug = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultLocale = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeZone = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPlatformAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailVerifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutUntil = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Slug = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressLine1 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressLine2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Region = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    TimeZone = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublishedVersion = table.Column<int>(type: "int", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.UniqueConstraint("AK_Buildings_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Buildings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LocationCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSearchableDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationCategories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSystem = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantSettings",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    DistanceUnit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultWalkingSpeedMps = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    SettingsJson = table.Column<string>(type: "json", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_TenantSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FileAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Kind = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StorageKey = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalFileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSanitized = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAssets", x => x.Id);
                    table.UniqueConstraint("AK_FileAssets_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_FileAssets_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileAssets_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    UserId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    TokenHash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FamilyId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    CreatedIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BuildingSettings",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    WalkingSpeedMps = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    DefaultLiftWaitSeconds = table.Column<int>(type: "int", nullable: false),
                    StairSecondsPerFloor = table.Column<int>(type: "int", nullable: false),
                    AllowManualStartSelection = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingSettings", x => x.BuildingId);
                    table.CheckConstraint("CK_BuildingSettings_Speed", "`WalkingSpeedMps` > 0");
                    table.ForeignKey(
                        name: "FK_BuildingSettings_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    LevelIndex = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ElevationMeters = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                    table.UniqueConstraint("AK_Floors_BuildingId_Id", x => new { x.BuildingId, x.Id });
                    table.UniqueConstraint("AK_Floors_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Floors_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GraphSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SnapshotJson = table.Column<string>(type: "json", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NodeCount = table.Column<int>(type: "int", nullable: false),
                    EdgeCount = table.Column<int>(type: "int", nullable: false),
                    PublishedByUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphSnapshots_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GraphSnapshots_Users_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VerticalConnectors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConnectorType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAccessible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRestricted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WaitSeconds = table.Column<int>(type: "int", nullable: true),
                    SecondsPerFloor = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerticalConnectors", x => x.Id);
                    table.UniqueConstraint("AK_VerticalConnectors_BuildingId_Id", x => new { x.BuildingId, x.Id });
                    table.ForeignKey(
                        name: "FK_VerticalConnectors_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    PermissionId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    UserId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    RoleId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvitedByUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMemberships", x => x.Id);
                    table.UniqueConstraint("AK_TenantMemberships_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Users_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantBranding",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoAssetId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    FaviconAssetId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    PrimaryColor = table.Column<string>(type: "char(7)", fixedLength: true, maxLength: 7, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondaryColor = table.Column<string>(type: "char(7)", fixedLength: true, maxLength: 7, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WelcomeMessage = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantBranding", x => x.TenantId);
                    table.CheckConstraint("CK_TenantBranding_Colors", "`PrimaryColor` LIKE '#%' AND `SecondaryColor` LIKE '#%'");
                    table.ForeignKey(
                        name: "FK_TenantBranding_FileAssets_TenantId_FaviconAssetId",
                        columns: x => new { x.TenantId, x.FaviconAssetId },
                        principalTable: "FileAssets",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBranding_FileAssets_TenantId_LogoAssetId",
                        columns: x => new { x.TenantId, x.LogoAssetId },
                        principalTable: "FileAssets",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantBranding_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FloorMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    FloorId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    FileAssetId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ViewBoxMinX = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ViewBoxMinY = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ViewBoxWidth = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ViewBoxHeight = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MetersPerUnit = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    ActiveFloorId = table.Column<Guid>(type: "binary(16)", nullable: true, computedColumnSql: "(CASE WHEN `IsActive` THEN `FloorId` ELSE NULL END)", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorMaps", x => x.Id);
                    table.CheckConstraint("CK_FloorMaps_Scale", "`MetersPerUnit` > 0");
                    table.ForeignKey(
                        name: "FK_FloorMaps_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FloorMaps_FileAssets_TenantId_FileAssetId",
                        columns: x => new { x.TenantId, x.FileAssetId },
                        principalTable: "FileAssets",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FloorMaps_Floors_BuildingId_FloorId",
                        columns: x => new { x.BuildingId, x.FloorId },
                        principalTable: "Floors",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FloorMaps_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    FloorId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Keywords = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LabelX = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    LabelY = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    ShapeJson = table.Column<string>(type: "json", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSearchable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.UniqueConstraint("AK_Locations_BuildingId_Id", x => new { x.BuildingId, x.Id });
                    table.UniqueConstraint("AK_Locations_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Locations_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_Floors_BuildingId_FloorId",
                        columns: x => new { x.BuildingId, x.FloorId },
                        principalTable: "Floors",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_LocationCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "LocationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NavigationNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    FloorId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    Code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NodeType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    X = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Y = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    LocationId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    ConnectorId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    IsAccessible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationNodes", x => x.Id);
                    table.UniqueConstraint("AK_NavigationNodes_BuildingId_Id", x => new { x.BuildingId, x.Id });
                    table.ForeignKey(
                        name: "FK_NavigationNodes_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationNodes_Floors_BuildingId_FloorId",
                        columns: x => new { x.BuildingId, x.FloorId },
                        principalTable: "Floors",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationNodes_Locations_BuildingId_LocationId",
                        columns: x => new { x.BuildingId, x.LocationId },
                        principalTable: "Locations",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationNodes_VerticalConnectors_BuildingId_ConnectorId",
                        columns: x => new { x.BuildingId, x.ConnectorId },
                        principalTable: "VerticalConnectors",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NavigationEdges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    FromNodeId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    ToNodeId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    EdgeType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsBidirectional = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DistanceMeters = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CostOverrideSeconds = table.Column<int>(type: "int", nullable: true),
                    IsAccessible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiresStairs = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRestricted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationEdges", x => x.Id);
                    table.CheckConstraint("CK_NavigationEdges_Distance", "`DistanceMeters` >= 0");
                    table.CheckConstraint("CK_NavigationEdges_NoSelfLoop", "`FromNodeId` <> `ToNodeId`");
                    table.ForeignKey(
                        name: "FK_NavigationEdges_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationEdges_NavigationNodes_BuildingId_FromNodeId",
                        columns: x => new { x.BuildingId, x.FromNodeId },
                        principalTable: "NavigationNodes",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationEdges_NavigationNodes_BuildingId_ToNodeId",
                        columns: x => new { x.BuildingId, x.ToNodeId },
                        principalTable: "NavigationNodes",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QrCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "binary(16)", nullable: false),
                    BuildingId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    NodeId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    LocationId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    Token = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Purpose = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisabledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DisabledReason = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastScannedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "binary(16)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodes", x => x.Id);
                    table.UniqueConstraint("AK_QrCodes_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_QrCodes_Buildings_TenantId_BuildingId",
                        columns: x => new { x.TenantId, x.BuildingId },
                        principalTable: "Buildings",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrCodes_Locations_BuildingId_LocationId",
                        columns: x => new { x.BuildingId, x.LocationId },
                        principalTable: "Locations",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrCodes_NavigationNodes_BuildingId_NodeId",
                        columns: x => new { x.BuildingId, x.NodeId },
                        principalTable: "NavigationNodes",
                        principalColumns: new[] { "BuildingId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QrCodes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QrScanEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    QrCodeId = table.Column<Guid>(type: "binary(16)", nullable: false),
                    ScannedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VisitorSessionId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgentFamily = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrScanEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrScanEvents_QrCodes_TenantId_QrCodeId",
                        columns: x => new { x.TenantId, x.QrCodeId },
                        principalTable: "QrCodes",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_TenantId_Slug",
                table: "Buildings",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_TenantId_Status",
                table: "Buildings",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingSettings_TenantId_BuildingId",
                table: "BuildingSettings",
                columns: new[] { "TenantId", "BuildingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_StorageKey",
                table: "FileAssets",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_TenantId_Kind",
                table: "FileAssets",
                columns: new[] { "TenantId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_FileAssets_UploadedByUserId",
                table: "FileAssets",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_ActiveFloorId",
                table: "FloorMaps",
                column: "ActiveFloorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_BuildingId_FloorId",
                table: "FloorMaps",
                columns: new[] { "BuildingId", "FloorId" });

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_CreatedByUserId",
                table: "FloorMaps",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_FloorId_Version",
                table: "FloorMaps",
                columns: new[] { "FloorId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_TenantId_BuildingId",
                table: "FloorMaps",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_FloorMaps_TenantId_FileAssetId",
                table: "FloorMaps",
                columns: new[] { "TenantId", "FileAssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Floors_BuildingId_LevelIndex",
                table: "Floors",
                columns: new[] { "BuildingId", "LevelIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Floors_TenantId_BuildingId",
                table: "Floors",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_GraphSnapshots_BuildingId_Version",
                table: "GraphSnapshots",
                columns: new[] { "BuildingId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphSnapshots_PublishedByUserId",
                table: "GraphSnapshots",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphSnapshots_TenantId_BuildingId",
                table: "GraphSnapshots",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationCategories_TenantId_Code",
                table: "LocationCategories",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_BuildingId_FloorId",
                table: "Locations",
                columns: new[] { "BuildingId", "FloorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_BuildingId_Status_IsSearchable",
                table: "Locations",
                columns: new[] { "BuildingId", "Status", "IsSearchable" });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CategoryId",
                table: "Locations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Name_Keywords",
                table: "Locations",
                columns: new[] { "Name", "Keywords" })
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_TenantId_BuildingId",
                table: "Locations",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEdges_BuildingId_FromNodeId",
                table: "NavigationEdges",
                columns: new[] { "BuildingId", "FromNodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEdges_BuildingId_IsActive",
                table: "NavigationEdges",
                columns: new[] { "BuildingId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEdges_BuildingId_ToNodeId",
                table: "NavigationEdges",
                columns: new[] { "BuildingId", "ToNodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEdges_FromNodeId_ToNodeId",
                table: "NavigationEdges",
                columns: new[] { "FromNodeId", "ToNodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEdges_TenantId_BuildingId",
                table: "NavigationEdges",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_BuildingId_Code",
                table: "NavigationNodes",
                columns: new[] { "BuildingId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_BuildingId_ConnectorId",
                table: "NavigationNodes",
                columns: new[] { "BuildingId", "ConnectorId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_BuildingId_FloorId",
                table: "NavigationNodes",
                columns: new[] { "BuildingId", "FloorId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_BuildingId_LocationId",
                table: "NavigationNodes",
                columns: new[] { "BuildingId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_ConnectorId",
                table: "NavigationNodes",
                column: "ConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_LocationId",
                table: "NavigationNodes",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationNodes_TenantId_BuildingId",
                table: "NavigationNodes",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_BuildingId_LocationId",
                table: "QrCodes",
                columns: new[] { "BuildingId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_BuildingId_NodeId",
                table: "QrCodes",
                columns: new[] { "BuildingId", "NodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_BuildingId_Status",
                table: "QrCodes",
                columns: new[] { "BuildingId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_CreatedByUserId",
                table: "QrCodes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_NodeId",
                table: "QrCodes",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_TenantId_BuildingId",
                table: "QrCodes",
                columns: new[] { "TenantId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_Token",
                table: "QrCodes",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QrScanEvents_QrCodeId_ScannedAt",
                table: "QrScanEvents",
                columns: new[] { "QrCodeId", "ScannedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QrScanEvents_TenantId_QrCodeId",
                table: "QrScanEvents",
                columns: new[] { "TenantId", "QrCodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_QrScanEvents_TenantId_ScannedAt",
                table: "QrScanEvents",
                columns: new[] { "TenantId", "ScannedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FamilyId",
                table: "RefreshTokens",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Code",
                table: "Roles",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantBranding_TenantId_FaviconAssetId",
                table: "TenantBranding",
                columns: new[] { "TenantId", "FaviconAssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantBranding_TenantId_LogoAssetId",
                table: "TenantBranding",
                columns: new[] { "TenantId", "LogoAssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_InvitedByUserId",
                table: "TenantMemberships",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_RoleId",
                table: "TenantMemberships",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_TenantId_UserId",
                table: "TenantMemberships",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantMemberships_UserId",
                table: "TenantMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Status",
                table: "Tenants",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerticalConnectors_BuildingId_Code",
                table: "VerticalConnectors",
                columns: new[] { "BuildingId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerticalConnectors_TenantId_BuildingId",
                table: "VerticalConnectors",
                columns: new[] { "TenantId", "BuildingId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BuildingSettings");

            migrationBuilder.DropTable(
                name: "FloorMaps");

            migrationBuilder.DropTable(
                name: "GraphSnapshots");

            migrationBuilder.DropTable(
                name: "NavigationEdges");

            migrationBuilder.DropTable(
                name: "QrScanEvents");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TenantBranding");

            migrationBuilder.DropTable(
                name: "TenantMemberships");

            migrationBuilder.DropTable(
                name: "TenantSettings");

            migrationBuilder.DropTable(
                name: "QrCodes");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "FileAssets");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "NavigationNodes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "VerticalConnectors");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropTable(
                name: "LocationCategories");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
