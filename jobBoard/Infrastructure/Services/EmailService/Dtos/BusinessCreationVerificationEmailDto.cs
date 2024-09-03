namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record BusinessCreationVerificationEmailDto(
    Guid UserId,
    string UserEmail,
    string BusinessName,
    string Token,
    int ExpiryInMinutes
);