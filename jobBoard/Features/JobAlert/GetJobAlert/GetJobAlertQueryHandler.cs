using JobBoard.Common.Models;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobAlert.GetJobAlert;

public class GetJobAlertQueryHandler(
    AppDbContext dbContext
) : IRequestHandler<GetJobAlertQuery, Result<GetJobAlertResponse, Error>>
{
    public async Task<Result<GetJobAlertResponse, Error>> Handle(GetJobAlertQuery query,
        CancellationToken cancellationToken)
    {
        var jobAlert = await dbContext.JobAlerts
            .Where(ja => ja.Token == query.Token)
            .Select(ja => new GetJobAlertResponse(
                ja.Email,
                ja.Keyword,
                ja.CountryId,
                ja.ExperienceLevels.Select(el => el.Id).ToList(),
                ja.EmploymentTypes.Select(et => et.Id).ToList(),
                ja.Categories.Select(c => c.Id).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (jobAlert is null) return JobAlertError.JobAlertNotFound;

        return jobAlert;
    }
}