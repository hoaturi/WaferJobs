using JobBoard.Infrastructure.Services.CachingServices.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobCities;

public record GetActiveJobCitiesResponse(IReadOnlyList<CityDto> Cities);