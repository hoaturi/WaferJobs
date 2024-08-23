using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Conference;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;

namespace JobBoard.Features.Conference.SubmitConference;

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
            StartDate = DateTime.SpecifyKind(DateTime.Parse(command.StartDate), DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(DateTime.Parse(command.EndDate), DateTimeKind.Utc),
            IsVerified = false,
            IsPublished = false
        };

        dbContext.Conferences.Add(newConference);
        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x =>
            x.SendConferenceSubmissionReviewAsync(new ConferenceSubmissionReviewDto(newConference.Title)));

        logger.LogInformation("A new conference has been submitted with ID: {ConferenceId}", newConference.Id);

        return Unit.Value;
    }
}