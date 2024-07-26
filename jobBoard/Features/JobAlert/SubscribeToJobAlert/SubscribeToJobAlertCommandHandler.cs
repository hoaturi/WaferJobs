using JobBoard.Common.Models;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Features.JobAlert.SubscribeToJobAlert;

public class SubscribeToJobAlertCommandHandler(
    AppDbContext dbContext,
    ILogger<SubscribeToJobAlertCommandHandler> logger)
    : IRequestHandler<SubscribeToJobAlertCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(SubscribeToJobAlertCommand command,
        CancellationToken cancellationToken)
    {
        await dbContext.JobAlerts.Upsert(new JobAlertEntity
            {
                EmailAddress = command.Email,
                Keyword = command.Keyword,
                CountryId = command.CountryId,
                EmploymentTypeId = command.EmploymentTypeId,
                CategoryId = command.CategoryId,
                Token = Base64UrlEncoder.Encode(Guid.NewGuid().ToString()),
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            })
            .On(x => new { x.EmailAddress })
            .WhenMatched(ja => new JobAlertEntity
            {
                EmailAddress = ja.EmailAddress,
                Keyword = ja.Keyword,
                CountryId = command.CountryId,
                EmploymentTypeId = command.EmploymentTypeId,
                CategoryId = command.CategoryId,
                UpdatedAt = DateTime.UtcNow,
                Token = ja.Token,
                CreatedAt = ja.CreatedAt
            })
            .RunAsync(cancellationToken);

        logger.LogInformation("Job alert for email {Email} successfully created or updated", command.Email);

        return Unit.Value;
    }
}