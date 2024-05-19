using System.Web;
using Azure;
using Azure.Communication.Email;
using JobBoard.Common.Interfaces;
using JobBoard.Common.Options;
using Microsoft.Extensions.Options;

namespace JobBoard.Infrastructure.Services.EmailService;

public class EmailService(
    IOptions<AzureOptions> azureOptions,
    IOptions<EmailOptions> emailOptions,
    ILogger<EmailService> logger)
    : IEmailService
{
    private readonly EmailClient _emailClient = new(azureOptions.Value.CommunicationServiceConnectionString);
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task SendAsync(PasswordResetDto passwordResetDto)
    {
        var encodedToken = HttpUtility.UrlEncode(passwordResetDto.Token);
        var passwordResetLink =
            _emailOptions.GetPasswordResetLink(encodedToken, passwordResetDto.UserEntity.Id.ToString());

        var emailContent = GeneratePasswordResetEmailContent(passwordResetLink);

        try
        {
            await _emailClient.SendAsync(
                WaitUntil.Started,
                _emailOptions.FromAddress,
                passwordResetDto.UserEntity.Email,
                "Reset Password",
                emailContent);

            logger.LogInformation("Password reset email sent successfully for user: {UserId}",
                passwordResetDto.UserEntity.Id);
        }
        catch (RequestFailedException ex)
        {
            logger.LogError("Failed to send password reset email. Error Code: {ErrorCode}", ex.ErrorCode);
            throw new EmailSendFailedException();
        }
    }

    private static string GeneratePasswordResetEmailContent(string passwordResetLink)
    {
        return $"""
                            <p>Please click the button below to reset your password.</p>
                            <p><a href='{passwordResetLink}'>Reset Password</a></p>
                            <p>If you did not request a password reset, please ignore this email or contact support if you have questions.</p>
                            <p>Thank you,<br>JobBoard Team</p>
                """;
    }
}