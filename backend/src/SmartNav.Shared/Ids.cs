namespace SmartNav.Shared;

/// <summary>
/// Central ID factory. UUIDv7 = time-ordered (good InnoDB insert locality) and
/// non-guessable (defence-in-depth against IDOR). Stored as BINARY(16) in MySQL.
/// </summary>
public static class Ids
{
    public static Guid New() => Guid.CreateVersion7();
}
