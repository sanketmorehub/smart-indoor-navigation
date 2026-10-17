using SmartNav.Domain.Common;

namespace SmartNav.Domain.Files;

/// <summary>Metadata for a blob in object storage. Binary content never lives in MySQL.</summary>
public sealed class FileAsset : TenantEntity
{
    public FileAssetKind Kind { get; set; }
    /// <summary>e.g. tenants/{tid}/buildings/{bid}/floors/{fid}/maps/{uuid}.svg - immutable, never overwritten.</summary>
    public required string StorageKey { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public required string Sha256 { get; set; }
    public string? OriginalFileName { get; set; }
    public bool IsSanitized { get; set; }
    public Guid? UploadedByUserId { get; set; }
}
