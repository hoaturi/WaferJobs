using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
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
        UpdateMyBusinessCommand command,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetUserId();

        var business = await appDbContext
            .Businesses
            .FirstOrDefaultAsync(b => b.Members.Any(m => m.UserId == currentUserId), cancellationToken);

        if (business is null)
        {
            logger.LogWarning("Business not found for user with id {UserId}", currentUserId);
            throw new BusinessNotFoundForUserException(currentUserId);
        }

        business.Name = command.Name;
        business.Description = command.Description;
        business.Location = command.Location;
        business.BusinessSizeId = command.BusinessSizeId;
        business.WebsiteUrl = command.WebsiteUrl;
        business.TwitterUrl = command.TwitterUrl;
        business.LinkedinUrl = command.LinkedInUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully updated business with id {BusinessId}", business.Id);

        return Unit.Value;
    }
}