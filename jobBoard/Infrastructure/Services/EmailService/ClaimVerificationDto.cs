namespace JobBoard.Infrastructure.Services.EmailService;

public record ClaimVerificationDto(string RecipientEmail, int Pin, string BusinessName);