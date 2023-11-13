using System.Text.Json;

namespace Core7Library.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// If the provided <see cref="json"/> is valid JSON, will return it in a readable format; else will return
    /// whatever the original string value was
    /// </summary>
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
