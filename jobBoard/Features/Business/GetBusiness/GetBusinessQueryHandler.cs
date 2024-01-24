using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class GetBusinessQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetBusinessQuery, Result<GetBusinessResponse, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetBusinessQuery request,
        CancellationToken cancellationToken
    )
    {
        var business = await _appDbContext
            .Businesses.AsNoTracking()
            .Where(b => b.Id == request.Id)
            .Select(
                b =>
                    new GetBusinessResponse
                    {
                        Id = b.Id,
                        Name = b.Name,
                        LogoUrl = b.LogoUrl,
                        Description = b.Description,
                        Location = b.Location,
                        Size = b.BusinessSize,
                        Url = b.Url,
                        TwitterUrl = b.TwitterUrl,
                        LinkedInUrl = b.LinkedInUrl,
                    }
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            return BusinessErrors.BusinessNotFound(request.Id);
        }

        return business;
    }
}
