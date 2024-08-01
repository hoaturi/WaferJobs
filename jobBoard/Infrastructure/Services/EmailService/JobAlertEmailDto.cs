using JobBoard.Infrastructure.BackgroundJobs.JobAlertSender;

namespace JobBoard.Infrastructure.Services.EmailService;

public record JobAlertEmailDto(
    string RecipientEmail,
    string? Keyword,
    string? Country,
    List<string>? Categories,
    List<string>? EmploymentTypes,
    string Token,
    string? FilterQuery,
    DateTime? LastSentAt,
    JobPostsWithCountDto JobPosts
);