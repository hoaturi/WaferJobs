namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record EmailChangeVerificationEmailDto(string NewEmail, string Pin);