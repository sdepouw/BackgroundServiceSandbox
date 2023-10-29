namespace Core7Library;

public static class CurrentEnvironment
{
    public static string Name { get; } = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? string.Empty;

    public static bool IsDevelopment => Name == "Development";
    public static bool IsProduction => Name == "Production";
}
