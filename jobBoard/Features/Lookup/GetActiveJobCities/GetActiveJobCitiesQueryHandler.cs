using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobCities;

public class GetActiveJobCitiesQueryHandler(
    ILocationService locationService)
    : IRequestHandler<GetActiveJobCitiesQuery, Result<GetActiveJobCitiesResponse, Error>>
{
    public async Task<Result<GetActiveJobCitiesResponse, Error>> Handle(GetActiveJobCitiesQuery request,
        CancellationToken cancellationToken)
    {
        var cities = await locationService.GetCitiesWithActiveJobPostAsync(cancellationToken);

        return new GetActiveJobCitiesResponse(cities);
    }
}