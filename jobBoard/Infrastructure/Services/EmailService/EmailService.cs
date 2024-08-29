using System.Web;
using JobBoard.Infrastructure.Options;
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

        logger.LogInformation("Email confirmation email sent to: {Email}", dto.RecipientEmail);
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

        logger.LogInformation("Password reset email sent to: {Email}", dto.User.Email);
    }

    public async Task SendEmailChangeVerificationAsync(EmailChangeVerificationDto dto)
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

        logger.LogInformation("Email change verification email sent to: {Email}", dto.NewEmail);
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
            totalCount = dto.JobPosts.TotalCount,
            jobPosts = dto.JobPosts.JobPosts
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);
    }

    public async Task SendBusinessClaimVerificationAsync(BusinessClaimVerificationEmailDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.BusinessClaimVerificationTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            businessName = dto.BusinessName,
            pin = dto.Pin
        };

        email.AddTo(new EmailAddress(dto.UserEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Business claim verification PIN sent for business '{BusinessName}' to user {UserId}",
            dto.BusinessName, dto.UserId);
    }

    public async Task SendConferenceSubmissionReviewAsync(ConferenceSubmissionReviewDto dto)
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

        logger.LogInformation("Conference submission review email sent to: {Email}", _emailOptions.SenderEmail);
    }

    public async Task SendBusinessMemberInvitationAsync(BusinessMemberInvitationDto dto)
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
            token = dto.Token
        };

        email.AddTo(new EmailAddress(dto.RecipientEmail));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Business member invitation email sent to: {Email}", dto.RecipientEmail);
    }

    public async Task SendPendingConferenceSubmissionReminderAsync(PendingConferenceSubmissionReminderDto dto)
    {
        var email = new SendGridMessage
        {
            From = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
            TemplateId = _sendGridOptions.PendingConferenceSubmissionReminderTemplateId
        };

        var templateData = new
        {
            baseUrl = _emailOptions.BaseUrl,
            conferences = dto.Conferences
        };

        email.AddTo(new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName));
        email.SetTemplateData(templateData);

        await emailClient.SendEmailAsync(email);

        logger.LogInformation("Pending conference submission reminder email sent to: {Email}",
            _emailOptions.SenderEmail);
    }
}