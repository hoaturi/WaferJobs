using System.Web;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace JobBoard;

public class EmailService : IEmailService
{
    private readonly AzureOptions _azureOptions;
    private readonly EmailOptions _emailOptions;
    private readonly EmailClient _emailClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<AzureOptions> azureOptions,
        IOptions<EmailOptions> emailOptions,
        ILogger<EmailService> logger
    )
    {
        _azureOptions = azureOptions.Value;
        _emailOptions = emailOptions.Value;
        _emailClient = new EmailClient(_azureOptions.CommunicationServiceConnectionString);
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(EmailDto dto)
    {
        var encodedToken = HttpUtility.UrlEncode(dto.Token);
        var url = $"{_emailOptions.ResetPasswordUrl}?token={encodedToken}/userId={dto.User.Id}";

        var htmlContent =
            $@"
            <p>Please click the button below to reset your password.</p>
            <p><a href='{url}'>Reset Password</a></p>
            <p>If you did not request a password reset, please ignore this email or contact support if you have questions.</p>
            <p>Thank you,<br>JobBoard Team</p>";

        try
        {
            await _emailClient.SendAsync(
                Azure.WaitUntil.Started,
                senderAddress: _emailOptions.FromAddress,
                recipientAddress: dto.User.Email,
                subject: "Reset Password",
                htmlContent: htmlContent
            );

            _logger.LogInformation(
                "Successfully sent password reset email for user: {UserId}",
                dto.User.Id
            );
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Failed to send password reset email: {errorCode}", ex.ErrorCode);
            throw new EmailSendFailedException();
        }
    }
}
