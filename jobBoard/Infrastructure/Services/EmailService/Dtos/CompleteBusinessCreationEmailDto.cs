namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record CompleteBusinessCreationEmailDto(
    Guid UserId,
    string UserEmail,
    string BusinessName,
    string Token,
    int ExpiryInMinutes
);