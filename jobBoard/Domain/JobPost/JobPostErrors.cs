using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.JobPost;

public static class JobPostErrors
{
    public static Error JobPostNotFound(Guid id)
    {
        return new Error(
            ErrorCodes.JobPostNotFound,
            HttpStatusCode.NotFound,
            $"Job post with id: {id} not found."
        );
    }
}