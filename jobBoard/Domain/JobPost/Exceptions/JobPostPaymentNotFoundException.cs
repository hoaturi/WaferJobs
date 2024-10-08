using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost.Exceptions;

public class JobPostPaymentNotFoundException(string sessionId)
    : CustomException(
        ErrorCodes.JobPostNotFound,
        HttpStatusCode.NotFound,
        $"Job post payment with session {sessionId} not found."
    );