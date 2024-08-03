using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.LookupServices.JobPostCountService;
using MediatR;

namespace JobBoard.Features.Lookup.GetJobPostCount;

public class GetJobPostCountQueryHandler(IJobPostCountService jobPostCountService)
    : IRequestHandler<GetJobPostCountQuery, Result<GetJobPostCountResponse, Error>>
{
    public async Task<Result<GetJobPostCountResponse, Error>> Handle(GetJobPostCountQuery request,
        CancellationToken cancellationToken)
    {
        var count = await jobPostCountService.GetJobPostCountAsync(cancellationToken);
        return new GetJobPostCountResponse(count);
    }
}