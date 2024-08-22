using MessagePack;

namespace JobBoard.Infrastructure.Services.CachingServices.LocationService;

[MessagePackObject]
public record CityDto(
    [property: Key(0)] int Id,
    [property: Key(1)] string Label,
    [property: Key(2)] string Slug
);