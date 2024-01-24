using System.Net;

namespace JobBoard;

public class EmailSendFailedException()
    : CustomException(
        ErrorCodes.EmailSendFailed,
        HttpStatusCode.InternalServerError,
        "Failed to send email."
    );
