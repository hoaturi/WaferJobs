using JobBoard.Common.Models;
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
                           .Where(b => b.Memberships.Any(m => m.UserId == currentUserId && m.IsActive))
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new BusinessNotFoundException(currentUserId);

        var membership = business.Memberships.First(m => m.UserId == currentUserId);
        if (!membership.IsAdmin) throw new BusinessMemberNotAdminException(business.Id, currentUserId);

        business.Name = command.Name;
        business.Description = command.Description;
        business.Location = command.Location;
        business.BusinessSizeId = command.BusinessSizeId;
        business.WebsiteUrl = command.WebsiteUrl;
        business.TwitterUrl = command.TwitterUrl;
        business.LinkedinUrl = command.LinkedInUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated business {BusinessId} for user {UserId}", business.Id, currentUserId);

        return Unit.Value;
    }
}