using System.Web;
using JobBoard.Common.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace JobBoard.Infrastructure.Services.EmailService;

public class EmailService(
    ISendGridClient emailClient,
    IOptions<EmailOptions> emailOptions,
    IOptions<SendGridOptions> sendGridOptions,
    ILogger<EmailService> logger)
    : IEmailService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly SendGridOptions _sendGridOptions = sendGridOptions.Value;

    public async Task SendPasswordResetAsync(PasswordResetEmailDto passwordResetEmailDto)
    {
        var encodedToken = HttpUtility.UrlEncode(passwordResetEmailDto.Token);


        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = "d-92fcef9ced2e4c0ca754cafa3e8cdbdd"
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            token = encodedToken,
            userId = passwordResetEmailDto.User.Id
        };

        email.AddTo(new EmailAddress(passwordResetEmailDto.User.Email));
        email.SetTemplateData(templateData);

        var response = await emailClient.SendEmailAsync(email);

        if (!response.IsSuccessStatusCode)
            logger.LogWarning("Failed to send password reset email to: {Email}. Status code: {StatusCode}",
                passwordResetEmailDto.User.Email, response.StatusCode);

        logger.LogInformation("Password reset email sent to: {Email}", passwordResetEmailDto.User.Email);
    }

    public async Task SendJobAlertAsync(JobAlertEmailDto jobAlertEmailDto)
    {
        var capitalizedKeyword = jobAlertEmailDto.Keyword is null
            ? string.Empty
            : char.ToUpper(jobAlertEmailDto.Keyword[0]) + jobAlertEmailDto.Keyword[1..];
        var joinedCategories = jobAlertEmailDto.Categories is { Count: > 0 }
            ? string.Join(", ", jobAlertEmailDto.Categories)
            : string.Empty;
        var joinedEmploymentTypes = jobAlertEmailDto.EmploymentTypes is { Count: > 0 }
            ? string.Join(", ", jobAlertEmailDto.EmploymentTypes)
            : string.Empty;

        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.JobAlertTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            keyword = capitalizedKeyword,
            country = jobAlertEmailDto.Country,
            categories = joinedCategories,
            employmentTypes = joinedEmploymentTypes,
            token = jobAlertEmailDto.Token,
            filterQuery = jobAlertEmailDto.FilterQuery,
            totalCount = jobAlertEmailDto.JobPosts.TotalCount,
            jobPosts = jobAlertEmailDto.JobPosts.JobPosts
        };

        email.AddTo(new EmailAddress(jobAlertEmailDto.RecipientEmail));
        email.SetTemplateData(templateData);

        var response = await emailClient.SendEmailAsync(email);

        if (!response.IsSuccessStatusCode)
            logger.LogWarning("Failed to send job alert email to: {Email}. Status code: {StatusCode}",
                jobAlertEmailDto.RecipientEmail, response.StatusCode);
    }
}