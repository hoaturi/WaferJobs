using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class TooManyVerificationAttemptsException(Guid userId) : CustomException(
    ErrorCodes.TooManyVerificationAttempts,
    HttpStatusCode.BadRequest,
    $"User {userId} has exceeded the maximum number of verification attempts."
);