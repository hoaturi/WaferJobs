using Hangfire;
using JobBoard.Common.Constants;
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

namespace JobBoard.Features.Business.BusinessClaim.InitiateBusinessClaim;

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
            await dbContext.Businesses.FirstOrDefaultAsync(x => x.Id == command.BusinessId, cancellationToken) ??
            throw new BusinessNotFoundException(command.BusinessId);

        var userEmailDomain = user.Email!.Split('@')[1];
        if (business.Domain != userEmailDomain) throw new EmailDomainMismatchException(userId, business.Id);

        var claimRequest = new BusinessClaimRequestEntity
        {
            BusinessId = business.Id,
            ClaimantUserId = userId,
            Pin = new Random().Next(PinConstants.MinValue, PinConstants.MaxValue),
            ExpiresAt = DateTime.UtcNow.AddMinutes(PinConstants.PinExpiryInMinutes),
            IsVerified = false,
            Attempts = 0
        };

        dbContext.BusinessClaimRequests.Add(claimRequest);
        await dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = new BusinessClaimVerificationEmailDto(
            user.Id,
            user.Email,
            business.Name,
            claimRequest.Pin);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessClaimVerificationAsync(emailDto));

        return Unit.Value;
    }
}