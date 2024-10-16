using Hangfire;
using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Conference;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Conference.SubmitConference;

public class SubmitConferenceCommandHandler(
    AppDbContext dbContext,
    IBackgroundJobClient backgroundJobClient,
    ILogger<SubmitConferenceCommand> logger)
    : IRequestHandler<SubmitConferenceCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(SubmitConferenceCommand command, CancellationToken cancellationToken)
    {
        var newConference = new ConferenceEntity
        {
            ContactEmail = command.ContactEmail,
            ContactName = command.ContactName,
            Title = command.Title,
            Organiser = command.Organiser,
            OrganiserEmail = command.OrganiserEmail,
            Location = command.Location,
            WebsiteUrl = command.WebsiteUrl,
            LogoUrl = command.LogoUrl,
            StartDate = DateTime.SpecifyKind(DateTime.Parse(command.StartDate), DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(DateTime.Parse(command.EndDate), DateTimeKind.Utc),
            IsVerified = false,
            IsPublished = false
        };

        dbContext.Conferences.Add(newConference);
        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x =>
            x.SendConferenceSubmissionReviewAsync(
                new ConferenceSubmissionReviewEmailDto(newConference.Id, newConference.Title)));

        logger.LogInformation("Conference {ConferenceId} submitted for review", newConference.Id);

        return Unit.Value;
    }
}