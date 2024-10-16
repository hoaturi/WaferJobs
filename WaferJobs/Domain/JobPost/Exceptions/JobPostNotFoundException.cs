using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.JobPost.Exceptions;

public class JobPostNotFoundException(Guid jobPostId) : CustomException(
    ErrorCodes.JobPostNotFound,
    HttpStatusCode.NotFound,
    $"Job post {jobPostId} not found."
);