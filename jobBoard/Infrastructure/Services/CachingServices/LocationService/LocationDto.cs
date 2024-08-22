using MessagePack;

namespace JobBoard.Infrastructure.Services.CachingServices.LocationService;

[MessagePackObject]
public record LocationDto(
    [property: Key(0)] string Label,
    [property: Key(1)] string? CitySlug,
    [property: Key(2)] string CountrySlug,
    [property: Key(3)] int? CityId,
    [property: Key(4)] int CountryId
);