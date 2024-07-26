using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class JobPostAlreadyDeletedException(Guid id) : CustomException(
    ErrorCodes.JobPostAlreadyDeleted,
    HttpStatusCode.BadRequest,
    $"Job post with id {id} is already deleted."
);