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
                    new GetBusinessResponse(
                        b.Id,
                        b.Name,
                        b.LogoUrl,
                        b.Description,
                        b.Location,
                        b.Size,
                        b.Url,
                        b.TwitterUrl,
                        b.LinkedInUrl
                    )
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            return BusinessErrors.BusinessNotFound(request.Id);
        }

        return business;
    }
}
