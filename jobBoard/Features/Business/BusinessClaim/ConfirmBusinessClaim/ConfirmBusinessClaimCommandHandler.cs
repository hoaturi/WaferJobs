using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Domain.Common.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.BusinessClaim.ConfirmBusinessClaim;

public class ConfirmBusinessClaimCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<ConfirmBusinessClaimCommandHandler> logger)
    : IRequestHandler<ConfirmBusinessClaimCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ConfirmBusinessClaimCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var claim = await dbContext.BusinessClaimRequests
                        .FirstOrDefaultAsync(
                            x => x.ClaimantUserId == userId && !x.IsVerified &&
                                 x.ExpiresAt > DateTime.UtcNow, cancellationToken) ??
                    throw new BusinessClaimRequestNotFoundException(userId);

        if (claim.Pin != command.Pin)
        {
            claim.Attempts++;
            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessErrors.InvalidClaimPin;
        }

        if (claim.Attempts >= PinConstants.MaxPinAttempts)
            throw new TooManyVerificationAttemptsException("business claim", userId);

        claim.IsVerified = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Business claim verified for business {BusinessId} by user {UserId}", claim.BusinessId,
            userId);

        return Unit.Value;
    }
}