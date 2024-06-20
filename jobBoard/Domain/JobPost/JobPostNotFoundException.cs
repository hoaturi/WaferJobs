using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class JobPostNotFoundException(Guid jobPostId) : CustomException(
    ErrorCodes.JobPostNotFoundError,
    HttpStatusCode.NotFound,
    $"Job post with id {jobPostId} not found."
);