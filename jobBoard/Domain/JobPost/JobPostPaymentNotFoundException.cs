using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.JobPost;

public class JobPostPaymentNotFoundException()
    : CustomException(
        ErrorCodes.JobPostNotFoundError,
        HttpStatusCode.NotFound,
        "Job post payment not found"
    );