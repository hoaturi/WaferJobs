using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost.Exceptions;

public class UnauthorizedJobPostAccessException(Guid userId, Guid jobPostId) : CustomException(
    ErrorCodes.UnauthorizedJobPostAccess,
    HttpStatusCode.Forbidden,
    $"User {userId} is not authorized to access job post {jobPostId}");