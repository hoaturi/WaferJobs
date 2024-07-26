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
            .FirstOrDefaultAsync(ja => ja.Token == query.Token, cancellationToken);

        if (jobAlert is null) return JobAlertError.JobAlertNotFound;

        return new GetJobAlertResponse(jobAlert.EmailAddress);
    }
}