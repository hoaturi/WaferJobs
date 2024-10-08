using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost.Exceptions;

public class JobPostNotFoundException(Guid jobPostId) : CustomException(
    ErrorCodes.JobPostNotFound,
    HttpStatusCode.NotFound,
    $"Job post {jobPostId} not found."
);