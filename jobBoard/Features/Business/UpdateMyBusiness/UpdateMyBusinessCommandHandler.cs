using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.UpdateMyBusiness;

public class UpdateMyBusinessCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUser,
    ILogger<UpdateMyBusinessCommandHandler> logger
) : IRequestHandler<UpdateMyBusinessCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        UpdateMyBusinessCommand query,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetUserId();

        var business = await appDbContext
            .Businesses
            .FirstOrDefaultAsync(b => b.UserId == currentUserId, cancellationToken);

        if (business is null)
        {
            logger.LogWarning("Business not found for user with id {UserId}", currentUserId);
            throw new BusinessNotFoundForUserException(currentUserId);
        }

        business.Name = query.Name;
        business.Description = query.Description;
        business.Location = query.Location;
        business.BusinessSizeId = query.BusinessSizeId;
        business.WebsiteUrl = query.Url;
        business.TwitterUrl = query.TwitterUrl;
        business.LinkedInUrl = query.LinkedInUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully updated business with id {BusinessId}", business.Id);

        return Unit.Value;
    }
}