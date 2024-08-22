using JobBoard.Infrastructure.Services.CachingServices.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public record GetActiveJobLocationsResponse(IReadOnlyList<LocationDto> Locations);