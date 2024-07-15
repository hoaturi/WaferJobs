using MessagePack;

namespace JobBoard.Infrastructure.Services.LocationService;

[MessagePackObject]
public record LocationDto(
    [property: Key(0)] string Label,
    [property: Key(1)] string? CitySlug,
    [property: Key(2)] string CountrySlug
);