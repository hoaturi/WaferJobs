using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Business.GetBusiness;

public class GetBusinessQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetBusinessQuery, Result<GetBusinessResponse, Error>>
{
    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetBusinessQuery query,
        CancellationToken cancellationToken)
    {
        var businessResponse = await dbContext
            .Businesses
            .AsNoTracking()
            .Where(b => b.Slug == query.Slug)
            .Select(b => new GetBusinessResponse(
                b.Id,
                b.Name,
                b.LogoUrl,
                b.Description,
                b.Location,
                b.WebsiteUrl,
                b.TwitterUrl,
                b.LinkedinUrl,
                b.BusinessSize != null ? b.BusinessSize.Label : null
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (businessResponse is not null) return businessResponse;

        return BusinessErrors.BusinessNotFound;
    }
}