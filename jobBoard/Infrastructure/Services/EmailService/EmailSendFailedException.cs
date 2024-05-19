using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Infrastructure.Services.EmailService;

public class EmailSendFailedException()
    : CustomException(
        ErrorCodes.EmailSendFailedError,
        HttpStatusCode.InternalServerError,
        "Failed to send email."
    );