namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record ConfirmEmailDto(string RecipientEmail, Guid UserId, string Token);