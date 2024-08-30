namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessClaimVerificationEmailDto(
    Guid UserId,
    string UserEmail,
    Guid BusinessId,
    string BusinessName,
    int Pin);