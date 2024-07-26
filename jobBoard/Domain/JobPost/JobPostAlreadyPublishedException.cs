using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class JobPostAlreadyPublishedException(Guid jobPostId) : CustomException(
    ErrorCodes.JobPostAlreadyPublished,
    HttpStatusCode.BadRequest,
    $"Job post with id {jobPostId} is already published."
);