using System.Text.Json;

namespace Core7Library.Extensions;

public static class StringExtensions
{
    public static string? TryFormatJson(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return json;
        try
        {
            var deserialized = JsonSerializer.Deserialize<dynamic>(json);
            return JsonSerializer.Serialize(deserialized, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return json;
        }
    }
}
