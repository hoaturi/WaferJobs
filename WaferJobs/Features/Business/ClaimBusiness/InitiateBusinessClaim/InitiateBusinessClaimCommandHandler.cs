using System.Web;
using Hangfire;
using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Auth.Exceptions;
using WaferJobs.Domain.Business;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public class InitiateBusinessClaimCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<InitiateBusinessClaimCommandHandler> logger
)
    : IRequestHandler<InitiateBusinessClaimCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InitiateBusinessClaimCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var business =
            await dbContext.Businesses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == command.BusinessId, cancellationToken) ??
            throw new BusinessNotFoundException(command.BusinessId);

        var isUserExistingMember = await dbContext.BusinessMemberships
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId, cancellationToken);

        if (isUserExistingMember) throw new DuplicateMembershipException(userId);

        var userEmailDomain = user.Email!.Split('@')[1];
        if (business.Domain != userEmailDomain) throw new EmailDomainMismatchException(userId, business.Id);

        var claimToken = new BusinessClaimTokenEntity
        {
            BusinessId = business.Id,
            UserId = userId,
            Token = Guid.NewGuid().ToBase64String(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(TokenConstants.ExpiresIn30Minutes)
        };

        dbContext.BusinessClaimTokens.Add(claimToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var encodedToken = HttpUtility.UrlEncode(claimToken.Token);

        var emailDto = new CompleteBusinessClaimEmailDto(
            user.Id,
            user.Email,
            business.Id,
            business.Name,
            encodedToken,
            TokenConstants.ExpiresIn30Minutes
        );

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendCompleteBusinessClaimAsync(emailDto));

        logger.LogInformation("User {UserId} initiated business claim for business {BusinessId}", userId, business.Id);
        return Unit.Value;
    }
}