using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;

namespace WaferJobs.Domain.JobPost;

public static class JobPostErrors
{
    public static Error JobPostNotFound()
    {
        return new Error(
            ErrorCodes.JobPostNotFound,
            HttpStatusCode.NotFound,
            "Job post not found."
        );
    }
}