using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class UnauthorizedJobPostAccessException(Guid jobPostId, Guid userId) : CustomException(
    ErrorCodes.UnauthorizedJobPostAccessError,
    HttpStatusCode.Unauthorized,
    $"User with id {userId} is not authorized to access job post with id {jobPostId}"
);