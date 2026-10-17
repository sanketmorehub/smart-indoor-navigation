using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Files;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Tenancy;

namespace SmartNav.Infrastructure.Persistence.Configurations;

/*
 * ISOLATION PATTERN: every child references its parent through a COMPOSITE foreign key
 * that includes TenantId (or BuildingId). The database itself then refuses e.g. a Floor
 * pointing at another tenant's Building, or an Edge joining nodes of two buildings -
 * even if application code has a bug.
 */

internal sealed class BuildingConfig : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> b)
    {
        b.ToTable("Buildings");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.Property(x => x.Slug).HasMaxLength(80);
        b.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.Status });
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.Country).HasMaxLength(2).IsFixedLength();
        b.Property(x => x.PostalCode).HasMaxLength(20);
        b.Property(x => x.TimeZone).HasMaxLength(64);
        b.Property(x => x.Latitude).HasPrecision(9, 6);
        b.Property(x => x.Longitude).HasPrecision(9, 6);
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId);
        b.HasOne(x => x.Settings).WithOne()
            .HasForeignKey<BuildingSettings>(s => new { s.TenantId, s.BuildingId })
            .HasPrincipalKey<Building>(x => new { x.TenantId, x.Id });
    }
}

internal sealed class BuildingSettingsConfig : IEntityTypeConfiguration<BuildingSettings>
{
    public void Configure(EntityTypeBuilder<BuildingSettings> b)
    {
        b.ToTable("BuildingSettings");
        b.HasKey(x => x.BuildingId);
        b.Property(x => x.WalkingSpeedMps).HasPrecision(3, 2);
        b.ToTable(t => t.HasCheckConstraint("CK_BuildingSettings_Speed", "`WalkingSpeedMps` > 0"));
    }
}

internal sealed class FloorConfig : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> b)
    {
        b.ToTable("Floors");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.HasAlternateKey(x => new { x.BuildingId, x.Id });
        b.HasIndex(x => new { x.BuildingId, x.LevelIndex }).IsUnique();
        b.Property(x => x.Code).HasMaxLength(20);
        b.Property(x => x.Name).HasMaxLength(100);
        b.Property(x => x.ElevationMeters).HasPrecision(7, 2);
        b.HasOne(x => x.Building).WithMany(x => x.Floors)
            .HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
    }
}

internal sealed class FloorMapConfig : IEntityTypeConfiguration<FloorMap>
{
    public void Configure(EntityTypeBuilder<FloorMap> b)
    {
        b.ToTable("FloorMaps");
        b.HasIndex(x => new { x.FloorId, x.Version }).IsUnique();
        b.Property(x => x.MetersPerUnit).HasPrecision(10, 6);

        // Only ONE active map per floor: generated column + unique index (NULLs don't collide).
        b.Property<Guid?>("ActiveFloorId")
            .HasComputedColumnSql("(CASE WHEN `IsActive` THEN `FloorId` ELSE NULL END)", stored: true);
        b.HasIndex("ActiveFloorId").IsUnique();

        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne<Floor>().WithMany().HasForeignKey(x => new { x.BuildingId, x.FloorId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        b.HasOne<FileAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.FileAssetId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId);
        b.ToTable(t => t.HasCheckConstraint("CK_FloorMaps_Scale", "`MetersPerUnit` > 0"));
    }
}

internal sealed class LocationCategoryConfig : IEntityTypeConfiguration<LocationCategory>
{
    public void Configure(EntityTypeBuilder<LocationCategory> b)
    {
        b.ToTable("LocationCategories");
        b.Property(x => x.Code).HasMaxLength(50);
        b.Property(x => x.Name).HasMaxLength(100);
        b.Property(x => x.Icon).HasMaxLength(50);
        b.Property(x => x.Color).HasMaxLength(7);
        b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId);
    }
}

internal sealed class LocationConfig : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> b)
    {
        b.ToTable("Locations");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.HasAlternateKey(x => new { x.BuildingId, x.Id });
        b.Property(x => x.ShortCode).HasMaxLength(30);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.Keywords).HasMaxLength(500);
        b.Property(x => x.ShapeJson).HasColumnType("json");
        b.HasIndex(x => new { x.BuildingId, x.Status, x.IsSearchable });
        b.HasIndex(x => new { x.BuildingId, x.FloorId });
        b.HasIndex(x => new { x.Name, x.Keywords }).IsFullText();

        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne(x => x.Floor).WithMany().HasForeignKey(x => new { x.BuildingId, x.FloorId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        // Category may be global (TenantId NULL) - the service layer validates tenant ownership.
        b.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
    }
}
