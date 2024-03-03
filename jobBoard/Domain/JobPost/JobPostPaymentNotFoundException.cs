using System.Net;

namespace JobBoard;

public class JobPostPaymentNotFoundException()
    : CustomException(
        ErrorCodes.JobPostNotFound,
        HttpStatusCode.NotFound,
        "Job post payment not found"
    );
