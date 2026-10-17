using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartNav.Domain.Files;
using SmartNav.Domain.Identity;
using SmartNav.Domain.Tenancy;

namespace SmartNav.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfig : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");
        b.Property(x => x.Slug).HasMaxLength(80);
        b.HasIndex(x => x.Slug).IsUnique();
        b.Property(x => x.OrganizationType).HasMaxLength(50);
        b.Property(x => x.DefaultLocale).HasMaxLength(10);
        b.Property(x => x.TimeZone).HasMaxLength(64);
        b.HasIndex(x => x.Status);
        b.HasOne(x => x.Branding).WithOne().HasForeignKey<TenantBranding>(x => x.TenantId);
        b.HasOne(x => x.Settings).WithOne().HasForeignKey<TenantSettings>(x => x.TenantId);
    }
}

internal sealed class TenantBrandingConfig : IEntityTypeConfiguration<TenantBranding>
{
    public void Configure(EntityTypeBuilder<TenantBranding> b)
    {
        b.ToTable("TenantBranding");
        b.HasKey(x => x.TenantId);
        b.Property(x => x.PrimaryColor).HasMaxLength(7).IsFixedLength();
        b.Property(x => x.SecondaryColor).HasMaxLength(7).IsFixedLength();
        b.Property(x => x.WelcomeMessage).HasMaxLength(500);
        // Logo/favicon must belong to the SAME tenant (composite FK).
        b.HasOne<FileAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.LogoAssetId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id });
        b.HasOne<FileAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.FaviconAssetId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id });
        b.ToTable(t => t.HasCheckConstraint("CK_TenantBranding_Colors",
            "`PrimaryColor` LIKE '#%' AND `SecondaryColor` LIKE '#%'"));
    }
}

internal sealed class TenantSettingsConfig : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> b)
    {
        b.ToTable("TenantSettings");
        b.HasKey(x => x.TenantId);
        b.Property(x => x.DistanceUnit).HasMaxLength(10);
        b.Property(x => x.DefaultWalkingSpeedMps).HasPrecision(3, 2);
        b.Property(x => x.SettingsJson).HasColumnType("json");
    }
}

internal sealed class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.Property(x => x.Email).HasMaxLength(254);
        b.Property(x => x.NormalizedEmail).HasMaxLength(254);
        b.HasIndex(x => x.NormalizedEmail).IsUnique();
        b.Property(x => x.PasswordHash).HasMaxLength(500);
        b.Property(x => x.SecurityStamp).HasMaxLength(64);
    }
}

internal sealed class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");
        b.Property(x => x.Code).HasMaxLength(50);
        b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId);
        b.HasMany(x => x.Permissions).WithOne().HasForeignKey(x => x.RoleId);
    }
}

internal sealed class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("Permissions");
        b.Property(x => x.Code).HasMaxLength(80);
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Description).HasMaxLength(300);
    }
}

internal sealed class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("RolePermissions");
        b.HasKey(x => new { x.RoleId, x.PermissionId });
        b.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
    }
}

internal sealed class TenantMembershipConfig : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> b)
    {
        b.ToTable("TenantMemberships");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();
        b.HasIndex(x => x.UserId);
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId);
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        b.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.InvitedByUserId);
    }
}

internal sealed class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => new { x.UserId, x.ExpiresAt });
        b.HasIndex(x => x.FamilyId);
        b.Property(x => x.CreatedIp).HasMaxLength(45);
        b.Property(x => x.UserAgent).HasMaxLength(300);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
    }
}

internal sealed class FileAssetConfig : IEntityTypeConfiguration<FileAsset>
{
    public void Configure(EntityTypeBuilder<FileAsset> b)
    {
        b.ToTable("FileAssets");
        b.HasAlternateKey(x => new { x.TenantId, x.Id });
        b.Property(x => x.StorageKey).HasMaxLength(400);
        b.HasIndex(x => x.StorageKey).IsUnique();
        b.Property(x => x.ContentType).HasMaxLength(100);
        b.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();
        b.Property(x => x.OriginalFileName).HasMaxLength(255);
        b.HasIndex(x => new { x.TenantId, x.Kind });
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId);
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UploadedByUserId);
    }
}
