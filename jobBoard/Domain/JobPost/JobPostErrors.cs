using System.Net;

namespace JobBoard;

public static class JobPostErrors
{
    public static Error JobPostNotFound(Guid Id) =>
        new(
            ErrorCodes.JobPostNotFound,
            HttpStatusCode.NotFound,
            $"Job post with id: {Id} not found."
        );
}
