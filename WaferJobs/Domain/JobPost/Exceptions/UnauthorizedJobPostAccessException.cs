using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.JobPost.Exceptions;

public class UnauthorizedJobPostAccessException(Guid userId, Guid jobPostId) : CustomException(
    ErrorCodes.UnauthorizedJobPostAccess,
    HttpStatusCode.Forbidden,
    $"User {userId} is not authorized to access job post {jobPostId}");