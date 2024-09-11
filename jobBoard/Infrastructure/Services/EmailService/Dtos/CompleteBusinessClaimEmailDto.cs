namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record CompleteBusinessClaimEmailDto(
    Guid UserId,
    string UserEmail,
    Guid BusinessId,
    string BusinessName,
    string Token,
    int ExpiryInMinutes
);