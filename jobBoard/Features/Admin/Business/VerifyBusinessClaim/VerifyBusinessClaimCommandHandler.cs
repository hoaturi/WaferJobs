using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Business.VerifyBusinessClaim;

public class VerifyBusinessClaimCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    IEmailService emailService,
    ILogger<VerifyBusinessClaimCommandHandler> logger)
    : IRequestHandler<VerifyBusinessClaimCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(VerifyBusinessClaimCommand command,
        CancellationToken cancellationToken)
    {
        var pendingClaim = await dbContext.BusinessClaimAttempts
            .Where(bc => bc.Id == command.ClaimId)
            .Include(bc => bc.Business)
            .Include(bc => bc.Business.Members)
            .FirstOrDefaultAsync(cancellationToken);

        if (pendingClaim is null) throw new BusinessClaimNotFoundException(command.ClaimId);
        if (pendingClaim.Business.IsClaimed) throw new BusinessAlreadyClaimedException(pendingClaim.Business.Id);
        if (pendingClaim.Status != ClaimStatus.Pending)
            throw new BusinessClaimAlreadyVerifiedException(pendingClaim.Id);

        var claimantMember = pendingClaim.Business.Members.FirstOrDefault(m => m.UserId == pendingClaim.ClaimantUserId);
        if (claimantMember is null) throw new BusinessClaimantNotFoundException(pendingClaim.ClaimantUserId);

        var verifierId = currentUserService.GetUserId();

        switch (command.Dto.Action)
        {
            case nameof(ClaimAction.Approve):
                pendingClaim.Status = ClaimStatus.Approved;
                pendingClaim.Business.IsClaimed = true;
                claimantMember.IsAdmin = true;
                claimantMember.JoinedAt = DateTime.UtcNow;
                claimantMember.IsVerified = true;
                logger.LogInformation(
                    "Business claim {ClaimId} for business {BusinessId} approved by verifier {VerifierId}. Claimant {ClaimantId} granted admin rights.",
                    pendingClaim.Id, pendingClaim.Business.Id, verifierId, claimantMember.UserId);
                break;

            case nameof(ClaimAction.Reject):
                pendingClaim.Status = ClaimStatus.Rejected;
                claimantMember.IsDeleted = true;
                logger.LogInformation(
                    "Business claim {ClaimId} for business {BusinessId} rejected by verifier {VerifierId}. Claimant {ClaimantId} removed.",
                    pendingClaim.Id, pendingClaim.Business.Id, verifierId, claimantMember.UserId);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        pendingClaim.VerifierId = verifierId;
        pendingClaim.VerificationDate = DateTime.UtcNow;
        pendingClaim.Notes = command.Dto.Notes;

        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue(() => emailService.SendBusinessClaimVerificationResultAsync(
            new BusinessClaimVerificationResultDto(
                pendingClaim.ClaimantEmail,
                pendingClaim.ClaimantFirstName,
                pendingClaim.Business.Name,
                pendingClaim.Status
            )));

        return Unit.Value;
    }
}