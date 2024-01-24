namespace JobBoard;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(EmailDto emailDto);
}
