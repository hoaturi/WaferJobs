using JobBoard.Common.Models;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobAlert.UnsubscribeToJobAlert;

public class UnsubscribeToJobAlertCommandHandler(
    AppDbContext dbContext,
    ILogger<UnsubscribeToJobAlertCommandHandler> logger)
    : IRequestHandler<UnsubscribeToJobAlertCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(UnsubscribeToJobAlertCommand command,
        CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.JobAlerts
            .Where(x => x.Token == command.Token)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount == 0)
        {
            logger.LogWarning("Unsubscribe attempt for job alert with token {UnsubscribeToken} failed: Alert not found",
                command.Token);
            return JobAlertError.JobAlertNotFound;
        }

        logger.LogInformation("Job alert with token {UnsubscribeToken} successfully unsubscribed",
            command.Token);

        return Unit.Value;
    }
}