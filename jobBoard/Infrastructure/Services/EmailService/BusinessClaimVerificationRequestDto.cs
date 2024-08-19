namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessClaimVerificationRequestDto(
    string RecipientEmail,
    string FirstName,
    string BusinessName
);