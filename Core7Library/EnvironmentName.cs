using Ardalis.SmartEnum;

namespace Core7Library;

public sealed class EnvironmentName : SmartEnum<EnvironmentName, string>
{
    public static readonly EnvironmentName Development = new(nameof(Development));
    public static readonly EnvironmentName Production = new(nameof(Production));

    private EnvironmentName(string name) : base(name, name) { }
}