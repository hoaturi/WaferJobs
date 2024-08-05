using JobBoard.Common.Models;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobAlert;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Persistence.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace JobBoard.Features.JobAlert.SubscribeToJobAlert;

public class SubscribeToJobAlertCommandHandler(
    AppDbContext dbContext,
    EntityConstraintChecker constraintChecker,
    ILogger<SubscribeToJobAlertCommandHandler> logger)
    : IRequestHandler<SubscribeToJobAlertCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(SubscribeToJobAlertCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var jobAlert = new JobAlertEntity
            {
                Email = command.Email,
                Keyword = command.Keyword?.ToLowerInvariant(),
                CountryId = command.CountryId,
                Categories = await GetCategories(command.CategoryIds, cancellationToken),
                EmploymentTypes = await GetEmploymentTypes(command.EmploymentTypeIds, cancellationToken),
                ExperienceLevels = await GetExperienceLevels(command.ExperienceLevelIds,cancellationToken),
                Token = Base64UrlEncoder.Encode(Guid.NewGuid().ToString())
            };

            dbContext.JobAlerts.Add(jobAlert);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Job alert with token {SubscribeToken} successfully subscribed", jobAlert.Token);

            return Unit.Value;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException pgEx ||
                !constraintChecker.IsUniqueConstraintViolation<JobAlertEntity>(dbContext, nameof(JobAlertEntity.Email),
                    pgEx.SqlState, pgEx.ConstraintName)) throw;

            logger.LogWarning("Job alert already exists for email {Email}, updating...", command.Email);

            dbContext.ChangeTracker.Clear();
            await UpdateExistingJobAlert(command, cancellationToken);
            return Unit.Value;
        }
    }

    private async Task UpdateExistingJobAlert(SubscribeToJobAlertCommand command, CancellationToken cancellationToken)
    {
        var existingJobAlert = await dbContext.JobAlerts
            .Include(ja => ja.Categories)
            .Include(ja => ja.EmploymentTypes)
            .Where(ja => ja.Email == command.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingJobAlert is not null)
        {
            existingJobAlert.Keyword = command.Keyword?.ToLowerInvariant();
            existingJobAlert.CountryId = command.CountryId;

            existingJobAlert.Categories.Clear();
            existingJobAlert.EmploymentTypes.Clear();

            existingJobAlert.Categories = await GetCategories(command.CategoryIds, cancellationToken);
            existingJobAlert.EmploymentTypes =
                await GetEmploymentTypes(command.EmploymentTypeIds, cancellationToken);

            dbContext.Update(existingJobAlert);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Job alert updated for email {Email}", command.Email);
        }
    }


    private async Task<List<EmploymentTypeEntity>> GetEmploymentTypes(List<int>? employmentTypeIds,
        CancellationToken cancellationToken)
    {
        if (employmentTypeIds is null || employmentTypeIds.Count == 0)
            return [];

        return await dbContext.EmploymentTypes
            .Where(et => employmentTypeIds.Contains(et.Id))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<CategoryEntity>> GetCategories(List<int>? categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds is null || categoryIds.Count == 0)
            return [];

        return await dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }
    
    private async Task<List<ExperienceLevelEntity>> GetExperienceLevels(List<int>? experienceLevelIds, CancellationToken cancellationToken)
    {
        if (experienceLevelIds is null || experienceLevelIds.Count == 0)
            return [];

        return await dbContext.ExperienceLevels
            .Where(el => experienceLevelIds.Contains(el.Id))
            .ToListAsync(cancellationToken);
    }
}