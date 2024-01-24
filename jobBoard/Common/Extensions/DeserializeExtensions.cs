using System.Text.Json;

namespace JobBoard;

public static class DeserializeExtensions
{
    private static readonly JsonSerializerOptions caseInsensitiveSerializerSettings =
        new() { PropertyNameCaseInsensitive = true };

    public static T DeserializeCaseInsensitive<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, caseInsensitiveSerializerSettings)!;
    }
}
