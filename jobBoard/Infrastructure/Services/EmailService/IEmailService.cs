namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendAsync(PasswordResetEmailDto passwordResetEmailDto);
}