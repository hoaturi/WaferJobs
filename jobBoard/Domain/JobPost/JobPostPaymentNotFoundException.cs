using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class JobPostPaymentNotFoundException(string sessionId)
    : CustomException(
        ErrorCodes.JobPostNotFoundError,
        HttpStatusCode.NotFound,
        $"Job post payment with session id {sessionId} not found."
    );