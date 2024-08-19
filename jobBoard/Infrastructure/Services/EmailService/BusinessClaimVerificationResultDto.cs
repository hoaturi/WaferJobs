using JobBoard.Common.Constants;

namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessClaimVerificationResultDto(
    string RecipientEmail,
    string RecipientFirstName,
    string BusinessName,
    ClaimStatus Status
);