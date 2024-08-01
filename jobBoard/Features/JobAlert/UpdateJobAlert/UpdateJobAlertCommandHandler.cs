using JobBoard.Common.Models;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public class UpdateJobAlertCommandHandler(
    AppDbContext dbContext,
    ILogger<UpdateJobAlertCommandHandler> logger)
    : IRequestHandler<UpdateJobAlertCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(UpdateJobAlertCommand request, CancellationToken cancellationToken)
    {
        var jobAlert = await dbContext.JobAlerts
            .Include(ja => ja.EmploymentTypes)
            .Include(ja => ja.Categories)
            .FirstOrDefaultAsync(ja => ja.Token == request.Token, cancellationToken);

        if (jobAlert is null)
        {
            logger.LogWarning("Update attempt for job alert with token {Token} failed: Alert not found", request.Token);
            return JobAlertError.JobAlertNotFound;
        }

        jobAlert.Keyword = request.Dto.Keyword?.ToLowerInvariant();
        jobAlert.CountryId = request.Dto.CountryId;

        if (request.Dto.EmploymentTypeIds is { Count: > 0 })
        {
            var employmentTypes = await dbContext.EmploymentTypes
                .Where(et => request.Dto.EmploymentTypeIds.Contains(et.Id))
                .ToListAsync(cancellationToken);

            jobAlert.EmploymentTypes.Clear();
            jobAlert.EmploymentTypes = employmentTypes;
        }

        if (request.Dto.CategoryIds is { Count: > 0 })
        {
            var categories = await dbContext.Categories
                .Where(c => request.Dto.CategoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            jobAlert.Categories.Clear();
            jobAlert.Categories = categories;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Job alert with token {Token} successfully updated", request.Token);
        return Unit.Value;
    }
}