using MessagePack;

namespace JobBoard.Infrastructure.Services.LookupServices.LocationService;

[MessagePackObject]
public record LocationDto(
    [property: Key(0)] string Label,
    [property: Key(1)] string? CitySlug,
    [property: Key(2)] string CountrySlug
);