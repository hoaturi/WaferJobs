using System.Net;

namespace JobBoard;

public class CreateSessionFailedException()
    : CustomException(
        ErrorCodes.CreateSessionFailed,
        HttpStatusCode.InternalServerError,
        "Failed to create session."
    );
