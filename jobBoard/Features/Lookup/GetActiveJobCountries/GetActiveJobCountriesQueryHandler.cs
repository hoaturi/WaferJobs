using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public class GetActiveJobCountriesQueryHandler(
    AppDbContext dbContext
) : IRequestHandler<GetActiveJobCountriesQuery, Result<GetActiveJobCountriesResponse, Error>>
{
    public async Task<Result<GetActiveJobCountriesResponse, Error>> Handle(GetActiveJobCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await dbContext.JobPosts
            .AsNoTracking()
            .Where(j => !j.IsDeleted && j.IsPublished)
            .Select(j => j.Country)
            .Distinct()
            .Select(c => new ActiveJobCountryDto(
                c.Id,
                c.Label,
                c.Slug))
            .ToListAsync(cancellationToken);

        return new GetActiveJobCountriesResponse(countries);
    }
}