using System.Web;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace JobBoard;

public class EmailService : IEmailService
{
    private readonly AzureOptions _azureOptions;
    private readonly EmailOptions _emailOptions;
    private readonly EmailClient _emailClient;

    public EmailService(IOptions<AzureOptions> azureOptions, IOptions<EmailOptions> emailOptions)
    {
        _azureOptions = azureOptions.Value;
        _emailOptions = emailOptions.Value;
        _emailClient = new EmailClient(_azureOptions.CommunicationServiceConnectionStrings);
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
        }
        catch (Exception)
        {
            throw new EmailSendFailedException();
        }
    }
}
