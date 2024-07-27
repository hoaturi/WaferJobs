using JobBoard.Common.Models;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public class UpdateJobAlertCommandHandler(
    AppDbContext dbContext,
    ILogger<UpdateJobAlertCommandHandler> logger) : IRequestHandler<UpdateJobAlertCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(UpdateJobAlertCommand request, CancellationToken cancellationToken)
    {
        var updatedCount = await dbContext.JobAlerts.Where(ja => ja.Token == request.Token).ExecuteUpdateAsync(
            setters =>
                setters.SetProperty(ja => ja.Keyword, request.Dto.Keyword)
                    .SetProperty(ja => ja.CountryId, request.Dto.CountryId)
                    .SetProperty(ja => ja.EmploymentTypeId, request.Dto.EmploymentTypeId)
                    .SetProperty(ja => ja.CategoryId, request.Dto.CategoryId)
            , cancellationToken);

        if (updatedCount == 0)
        {
            logger.LogWarning("Update attempt for job alert with token {Token} failed: Alert not found", request.Token);
            return JobAlertError.JobAlertNotFound;
        }

        logger.LogInformation("Job alert with token {Token} successfully updated", request.Token);
        return Unit.Value;
    }
}