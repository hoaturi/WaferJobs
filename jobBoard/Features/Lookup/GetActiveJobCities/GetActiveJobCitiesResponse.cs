namespace JobBoard.Features.Lookup.GetActiveJobCities;

public record ActiveJobCityDto(int Id, string Label, string Slug);

public record GetActiveJobCitiesResponse(List<ActiveJobCityDto> Cities);