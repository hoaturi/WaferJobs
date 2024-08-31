using System.Web;
using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public class InitiateBusinessClaimCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient
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

        var isUserExistingMember = await dbContext.BusinessMembers
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
            ExpiresAt = DateTime.UtcNow.AddMinutes(TokenConstants.BusinessTokenExpirationInMinutes)
        };

        dbContext.BusinessClaimTokens.Add(claimToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var encodedToken = HttpUtility.UrlEncode(claimToken.Token);

        var emailDto = new BusinessClaimVerificationEmailDto(
            user.Id,
            user.Email,
            business.Id,
            business.Name,
            encodedToken,
            TokenConstants.BusinessTokenExpirationInMinutes
        );

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessClaimVerificationAsync(emailDto));

        return Unit.Value;
    }
}