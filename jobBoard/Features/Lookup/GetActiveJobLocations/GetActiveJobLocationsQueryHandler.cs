﻿using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public class GetActiveJobLocationsQueryHandler(ILocationService locationService)
    : IRequestHandler<GetActiveJobLocationsQuery, Result<GetActiveJobLocationsResponse, Error>>
{
    public async Task<Result<GetActiveJobLocationsResponse, Error>> Handle(GetActiveJobLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await locationService.GetCountriesWithActiveJobPostAsync(cancellationToken);
        var cities = await locationService.GetCitiesWithActiveJobPostAsync(cancellationToken);

        return new GetActiveJobLocationsResponse(countries, cities);
    }
}