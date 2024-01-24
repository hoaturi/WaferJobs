using System.Net;

namespace JobBoard;

public static class JobPostErrors
{
    public static readonly Error JobPostNotFound =
        new(ErrorCodes.JobPostNotFound, HttpStatusCode.NotFound, $"Job post not found.");
}
