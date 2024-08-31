namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public record ActiveJobLocationDto(
    string Label,
    string? CitySlug,
    string CountrySlug,
    int? CityId,
    int CountryId
);

public record GetActiveJobLocationsResponse(List<ActiveJobLocationDto> Locations);