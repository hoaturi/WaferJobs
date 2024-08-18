namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendEmailConfirmAsync(ConfirmEmailDto confirmEmailDto);
    Task SendPasswordResetAsync(PasswordResetEmailDto passwordResetEmailDto);

    Task SendJobAlertAsync(JobAlertEmailDto jobAlertEmailDto);
}