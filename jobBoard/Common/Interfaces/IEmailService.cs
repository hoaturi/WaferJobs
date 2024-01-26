namespace JobBoard;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(EmailDto emailDto);
}
