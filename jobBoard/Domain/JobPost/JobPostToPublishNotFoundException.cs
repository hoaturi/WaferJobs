using System.Net;

namespace JobBoard;

public class JobPostToPublishNotFoundException()
    : CustomException(
        ErrorCodes.JobPostNotFound,
        HttpStatusCode.NotFound,
        "Job post to publish not found"
    );
