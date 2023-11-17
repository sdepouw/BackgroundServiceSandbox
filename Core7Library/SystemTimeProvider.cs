namespace Core7Library;

public class SystemTimeProvider : TimeProviderAbstraction
{
    public override DateTimeOffset GetLocalNow() => DateTimeOffset.Now;

    public override DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}
