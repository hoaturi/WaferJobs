using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Infrastructure.Services.EmailService;

public class EmailSendFailedException()
    : CustomException(
        ErrorCodes.EmailSendFailed,
        HttpStatusCode.InternalServerError,
        "Failed to send email."
    );