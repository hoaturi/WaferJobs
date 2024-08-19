using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Features.Business.GetBusiness;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
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
                .Businesses.Where(b => b.Members.Any(m => m.UserId == currentUserId))
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
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessNotFoundForUserException(currentUserId);

        return businessResponse;
    }
}