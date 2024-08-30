using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Common.Exceptions;

public class TooManyVerificationAttemptsException(string verificationType, Guid userId) : CustomException(
    ErrorCodes.TooManyVerificationAttempts,
    HttpStatusCode.BadRequest,
    $"User {userId} has exceeded the maximum number of verification attempts for {verificationType}."
);