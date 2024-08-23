namespace JobBoard.Infrastructure.Services.EmailService;

public record ConfirmEmailDto(string RecipientEmail, Guid UserId, string Token);