using System.Web;
using JobBoard.Infrastructure.Options;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
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

    public async Task SendEmailConfirmAsync(ConfirmEmailDto dto)
    {
        var encodedToken = HttpUtility.UrlEncode(dto.Token);

        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.ConfirmEmailTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            token = encodedToken,
            userId = dto.UserId
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent email confirmation email for user {UserId}", dto.UserId);
    }

    public async Task SendPasswordResetAsync(PasswordResetEmailDto dto)
    {
        var encodedToken = HttpUtility.UrlEncode(dto.Token);

        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.PasswordResetTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            token = encodedToken,
            userId = dto.User.Id
        };

        email.AddTo(new EmailAddress(dto.User.Email));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent password reset email for user {UserId}", dto.User.Id);
    }

    public async Task SendEmailChangeVerificationAsync(EmailChangeVerificationEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.EmailChangeConfirmationTemplateId
        };

        var templateData = new
        {
            pin = dto.Pin
        };

        email.AddTo(new EmailAddress(dto.NewEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent email change verification email for user {UserId}", dto.UserId);
    }

    public async Task SendJobAlertAsync(JobAlertEmailDto dto)
    {
        var capitalizedKeyword = dto.Keyword is null
            ? string.Empty
            : char.ToUpper(dto.Keyword[0]) + dto.Keyword[1..];
        var joinedCategories = dto.Categories is { Count: > 0 }
            ? string.Join(", ", dto.Categories)
            : string.Empty;
        var joinedEmploymentTypes = dto.EmploymentTypes is { Count: > 0 }
            ? string.Join(", ", dto.EmploymentTypes)
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
            country = dto.Country,
            categories = joinedCategories,
            employmentTypes = joinedEmploymentTypes,
            token = dto.Token,
            filterQuery = dto.FilterQuery,
            totalCount = dto.JobPosts.TotalMatchCount,
            jobPosts = dto.JobPosts.JobPosts
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);
    }

    public async Task SendCompleteBusinessClaimAsync(CompleteBusinessClaimEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.CompleteBusinessClaimTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            businessName = dto.BusinessName,
            token = dto.Token,
            expiry = $"{dto.ExpiryInMinutes} minutes"
        };

        email.AddTo(new EmailAddress(dto.UserEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent business claim verification email for user {UserId}", dto.UserId);
    }

    public async Task SendCompleteBusinessCreationAsync(CompleteBusinessCreationEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.CompleteBusinessCreationTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            businessName = dto.BusinessName,
            token = dto.Token,
            expiry = $"{dto.ExpiryInMinutes} minutes"
        };

        email.AddTo(new EmailAddress(dto.UserEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent business creation verification email for user {UserId}", dto.UserId);
    }

    public async Task SendBusinessCreationReviewAsync(BusinessCreationReviewEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.BusinessCreationReviewTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            businessName = dto.BusinessId
        };

        email.AddTo(new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent business creation review email for business {BusinessId}", dto.BusinessId);
    }

    public async Task SendBusinessReviewResultAsync(BusinessReviewResultEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = dto.IsApproved
                ? _sendGridOptions.BusinessReviewApprovedTemplateId
                : _sendGridOptions.BusinessReviewRejectedTemplateId
        };

        var templateData = new
        {
            baseUrl = dto.IsApproved ? _emailOptions.BaseUrl : null,
            businessName = dto.BusinessName
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent business review result email for business {BusinessId} to user {UserId}",
            dto.BusinessId, dto.UserId);
    }

    public async Task SendBusinessMemberInvitationAsync(BusinessMemberInvitationEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.BusinessMemberInvitationTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            businessName = dto.BusinessName,
            inviterName = dto.InviterName,
            token = dto.Token,
            expiry = $"{dto.Expiry} days"
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Sent business member invitation email by user {UserId} for business {BusinessId} ",
            dto.BusinessId, dto.InviterId);
    }

    public async Task SendConferenceSubmissionReviewAsync(ConferenceSubmissionReviewEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.ConferenceSubmissionReviewTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            title = dto.Title
        };

        email.AddTo(new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);
        logger.LogInformation("Sent conference submission review email for conference {ConferenceId}, {Title}",
            dto.ConferenceId, dto.Title);
    }
}