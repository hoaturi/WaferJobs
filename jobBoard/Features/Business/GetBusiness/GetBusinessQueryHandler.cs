using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.GetBusiness;

public class GetBusinessQueryHandler(
    AppDbContext appDbContext,
    ILogger<GetBusinessQueryHandler> logger)
    : IRequestHandler<GetBusinessQuery, Result<GetBusinessResponse, Error>>
{
    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetBusinessQuery query,
        CancellationToken cancellationToken)
    {
        var businessResponse = await appDbContext
            .Businesses
            .AsNoTracking()
            .Where(b => b.Id == query.Id)
            .Select(b => new GetBusinessResponse(
                b.Id,
                b.Name,
                b.LogoUrl,
                b.Description,
                b.Location,
                b.WebsiteUrl,
                b.TwitterUrl,
                b.LinkedinUrl,
                b.BusinessSize != null ? b.BusinessSize.Name : null
            ))
            .FirstOrDefaultAsync(cancellationToken);


        if (businessResponse is not null) return businessResponse;

        logger.LogWarning("Business with id {BusinessId} not found", query.Id);
        return BusinessErrors.BusinessNotFound(query.Id);
    }
}