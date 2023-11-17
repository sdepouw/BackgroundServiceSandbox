namespace Core7Library;

/// <summary>
/// Abstract class that mimics the TimeProvider abstraction in .NET 8.
/// </summary>
public abstract class TimeProviderAbstraction
{
    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value that is set to the current date and time according to this TimeProvider's
    /// notion of time based on <see cref="GetUtcNow"/>, with the offset set to the offset from Coordinated Universal Time (UTC).
    /// </summary>
    public abstract DateTimeOffset GetLocalNow();
    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value whose date and time are set to the current
    /// Coordinated Universal Time (UTC) date and time and whose offset is Zero,
    /// all according to this TimeProvider's notion of time.
    /// </summary>
    /// <remarks>
    /// The default implementation returns <see cref="DateTimeOffset.UtcNow"/>.
    /// </remarks>
    public abstract DateTimeOffset GetUtcNow();

    public static readonly TimeProviderAbstraction System = new SystemTimeProvider();
}
