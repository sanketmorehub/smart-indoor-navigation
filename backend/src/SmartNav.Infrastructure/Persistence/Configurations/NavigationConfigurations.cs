using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartNav.Domain.Audit;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Navigation;
using SmartNav.Domain.Qr;

namespace SmartNav.Infrastructure.Persistence.Configurations;

internal sealed class VerticalConnectorConfig : IEntityTypeConfiguration<VerticalConnector>
{
    public void Configure(EntityTypeBuilder<VerticalConnector> b)
    {
        b.ToTable("VerticalConnectors");
        b.HasAlternateKey(x => new { x.BuildingId, x.Id });
        b.Property(x => x.Code).HasMaxLength(30);
        b.Property(x => x.Name).HasMaxLength(100);
        b.HasIndex(x => new { x.BuildingId, x.Code }).IsUnique();
        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
    }
}

internal sealed class NavigationNodeConfig : IEntityTypeConfiguration<NavigationNode>
{
    public void Configure(EntityTypeBuilder<NavigationNode> b)
    {
        b.ToTable("NavigationNodes");
        b.HasAlternateKey(x => new { x.BuildingId, x.Id });
        b.Property(x => x.Code).HasMaxLength(30);
        b.HasIndex(x => new { x.BuildingId, x.Code }).IsUnique();
        b.HasIndex(x => new { x.BuildingId, x.FloorId });
        b.HasIndex(x => x.LocationId);
        b.HasIndex(x => x.ConnectorId);

        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne<Floor>().WithMany().HasForeignKey(x => new { x.BuildingId, x.FloorId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        // Nullable composite FKs: only enforced when the column is set.
        b.HasOne<Location>().WithMany().HasForeignKey(x => new { x.BuildingId, x.LocationId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        b.HasOne<VerticalConnector>().WithMany().HasForeignKey(x => new { x.BuildingId, x.ConnectorId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
    }
}

internal sealed class NavigationEdgeConfig : IEntityTypeConfiguration<NavigationEdge>
{
    public void Configure(EntityTypeBuilder<NavigationEdge> b)
    {
        b.ToTable("NavigationEdges");
        b.HasIndex(x => new { x.FromNodeId, x.ToNodeId }).IsUnique();
        b.HasIndex(x => new { x.BuildingId, x.IsActive });

        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        // Both ends MUST be in the same building as the edge.
        b.HasOne<NavigationNode>().WithMany().HasForeignKey(x => new { x.BuildingId, x.FromNodeId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        b.HasOne<NavigationNode>().WithMany().HasForeignKey(x => new { x.BuildingId, x.ToNodeId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });

        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_NavigationEdges_NoSelfLoop", "`FromNodeId` <> `ToNodeId`");
            t.HasCheckConstraint("CK_NavigationEdges_Distance", "`DistanceMeters` >= 0");
        });
    }
}

internal sealed class GraphSnapshotConfig : IEntityTypeConfiguration<GraphSnapshot>
{
    public void Configure(EntityTypeBuilder<GraphSnapshot> b)
    {
        b.ToTable("GraphSnapshots");
        b.HasIndex(x => new { x.BuildingId, x.Version }).IsUnique();
        b.Property(x => x.SnapshotJson).HasColumnType("json");
        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne<User>().WithMany().HasForeignKey(x => x.PublishedByUserId);
    }
}

internal sealed class QrCodeConfig : IEntityTypeConfiguration<QrCode>
{
    public void Configure(EntityTypeBuilder<QrCode> b)
    {
        b.ToTable("QrCodes");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.Property(x => x.Token).HasMaxLength(32);
        b.HasIndex(x => x.Token).IsUnique();
        b.Property(x => x.Label).HasMaxLength(150);
        b.Property(x => x.DisabledReason).HasMaxLength(300);
        b.HasIndex(x => new { x.BuildingId, x.Status });
        b.HasIndex(x => x.NodeId);

        b.HasOne<Building>().WithMany().HasForeignKey(x => new { x.TenantId, x.BuildingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        b.HasOne<NavigationNode>().WithMany().HasForeignKey(x => new { x.BuildingId, x.NodeId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        b.HasOne<Location>().WithMany().HasForeignKey(x => new { x.BuildingId, x.LocationId })
            .HasPrincipalKey(x => new { x.BuildingId, x.Id });
        b.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId);
    }
}

internal sealed class QrScanEventConfig : IEntityTypeConfiguration<QrScanEvent>
{
    public void Configure(EntityTypeBuilder<QrScanEvent> b)
    {
        b.ToTable("QrScanEvents");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.VisitorSessionId).HasMaxLength(64);
        b.Property(x => x.UserAgentFamily).HasMaxLength(50);
        b.HasIndex(x => new { x.TenantId, x.ScannedAt });
        b.HasIndex(x => new { x.QrCodeId, x.ScannedAt });
        b.HasOne<QrCode>().WithMany().HasForeignKey(x => new { x.TenantId, x.QrCodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
    }
}

internal sealed class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("AuditLogs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.Action).HasMaxLength(80);
        b.Property(x => x.EntityType).HasMaxLength(80);
        b.Property(x => x.ChangesJson).HasColumnType("json");
        b.Property(x => x.IpAddress).HasMaxLength(45);
        b.Property(x => x.UserAgent).HasMaxLength(300);
        b.Property(x => x.CorrelationId).HasMaxLength(64);
        b.HasIndex(x => new { x.TenantId, x.CreatedAt });
        b.HasIndex(x => new { x.EntityType, x.EntityId });
    }
}
