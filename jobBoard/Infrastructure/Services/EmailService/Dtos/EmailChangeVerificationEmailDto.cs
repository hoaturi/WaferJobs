namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record EmailChangeVerificationEmailDto(Guid UserId, string NewEmail, string Pin);