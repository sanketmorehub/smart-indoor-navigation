namespace SmartNav.Shared;

/// <summary>Abstracted clock so time is testable. Always UTC.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
