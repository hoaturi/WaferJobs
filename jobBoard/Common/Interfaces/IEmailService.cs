using JobBoard.Infrastructure.Services.EmailService;

namespace JobBoard.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(PasswordResetDto passwordResetDto);
}