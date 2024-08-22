using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.CachingServices.LocationService;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public class GetActiveJobCountriesQueryHandler(
    ILocationService locationService
) : IRequestHandler<GetActiveJobCountriesQuery, Result<GetActiveJobCountriesResponse, Error>>
{
    public async Task<Result<GetActiveJobCountriesResponse, Error>> Handle(GetActiveJobCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await locationService.GetCountriesWithActiveJobPostAsync(cancellationToken);

        return new GetActiveJobCountriesResponse(countries);
    }
}