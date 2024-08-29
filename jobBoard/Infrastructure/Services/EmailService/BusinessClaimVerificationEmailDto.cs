namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessClaimVerificationEmailDto(Guid UserId, string UserEmail, string BusinessName, int Pin);