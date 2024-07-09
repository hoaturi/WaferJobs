using MessagePack;

namespace JobBoard.Infrastructure.Services.LocationService;

[MessagePackObject]
public record CountryDto(
    [property: Key(0)] int Id,
    [property: Key(1)] string Label,
    [property: Key(2)] string Slug
);