namespace JobBoard.Infrastructure.Services.EmailService;

public record EmailChangeVerificationDto(string NewEmail, int Pin);