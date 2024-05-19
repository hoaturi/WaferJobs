using System.Text.Json;

namespace JobBoard.Common.Extensions;

public static class JsonDeserializeExtensions
{
    private static readonly JsonSerializerOptions CaseInsensitiveSerializerSettings =
        new() { PropertyNameCaseInsensitive = true };

    public static T DeserializeWithCaseInsensitivity<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, CaseInsensitiveSerializerSettings)!;
    }
}