using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Features.Business.GetBusiness;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.GetMyBusiness;

public class GetMyBusinessQueryHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMyBusinessQuery, Result<GetBusinessResponse, Error>>
{
    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetMyBusinessQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUserService.GetUserId();

        var businessResponse =
            await appDbContext
                .Businesses.Where(b => b.UserId == currentUserId)
                .Include(b => b.BusinessSize)
                .Select(b => new GetBusinessResponse(
                    b.Id,
                    b.Name,
                    b.LogoUrl,
                    b.Description,
                    b.Location,
                    b.WebsiteUrl,
                    b.TwitterUrl,
                    b.LinkedInUrl,
                    b.BusinessSize != null ? b.BusinessSize.Name : null
                ))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessNotFoundForUserException(currentUserId);

        return businessResponse;
    }
}