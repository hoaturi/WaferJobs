using System.Collections.Immutable;
using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobCities;

public record GetActiveJobCitiesResponse(ImmutableArray<CityDto> Cities);