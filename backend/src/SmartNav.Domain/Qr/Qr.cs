using SmartNav.Domain.Common;

namespace SmartNav.Domain.Qr;

/// <summary>
/// A printed QR anchor. Token is opaque random (128-bit, base62) - it reveals no IDs.
/// Floor is derived from the node, so they can never disagree. Never hard-deleted.
/// </summary>
public sealed class QrCode : TenantEntity
{
    public Guid BuildingId { get; set; }
    public Guid NodeId { get; set; }
    public Guid? LocationId { get; set; }
    public required string Token { get; set; }
    public required string Label { get; set; }
    public QrPurpose Purpose { get; set; } = QrPurpose.Start;
    public QrStatus Status { get; set; } = QrStatus.Active;
    public DateTime? DisabledAt { get; set; }
    public string? DisabledReason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastScannedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>Append-only, high-volume. Cheap to record now; analytics dashboards come later.</summary>
public sealed class QrScanEvent : ITenantOwned
{
    public long Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid QrCodeId { get; set; }
    public DateTime ScannedAt { get; set; }
    public string? VisitorSessionId { get; set; }
    public string? UserAgentFamily { get; set; }
}
