using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Infrastructure.Services.PaymentService;

public class CreateSessionFailedException()
    : CustomException(
        ErrorCodes.CreateSessionFailed,
        HttpStatusCode.InternalServerError,
        "Failed to create session."
    );