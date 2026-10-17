namespace SmartNav.Domain.Common;

// All enums are persisted as VARCHAR strings (never MySQL ENUM) so new values need no ALTER TABLE.
public enum RecordStatus { Active, Inactive }
public enum TenantStatus { Active, Suspended, Archived }
public enum UserStatus { Active, Disabled }
public enum MembershipStatus { Invited, Active, Disabled }
public enum BuildingStatus { Draft, Published, Archived }
public enum ConnectorType { Lift, Stair, Escalator, Ramp }
public enum ConnectorStatus { Active, Maintenance, Closed }
public enum NodeType { Waypoint, Junction, Entrance, Door, LocationAccess, Connector }
public enum EdgeType { Walk, Door, Ramp, Lift, Stair, Escalator }
public enum QrStatus { Active, Disabled, Revoked }
public enum QrPurpose { Start, Checkpoint, Transition }
public enum FileAssetKind { FloorMap, Logo, Favicon, Attachment }
public enum PermissionScope { Platform, Tenant }
public enum AuditActorType { User, PlatformAdmin, System }
