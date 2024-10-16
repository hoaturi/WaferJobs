using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.JobPost.Exceptions;

public class JobPostPaymentNotFoundException(string sessionId)
    : CustomException(
        ErrorCodes.JobPostNotFound,
        HttpStatusCode.NotFound,
        $"Job post payment with session {sessionId} not found."
    );