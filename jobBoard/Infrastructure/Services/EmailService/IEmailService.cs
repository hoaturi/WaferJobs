namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendPasswordResetAsync(PasswordResetEmailDto passwordResetEmailDto);

    Task SendJobAlertAsync(JobAlertEmailDto jobAlertEmailDto);
}