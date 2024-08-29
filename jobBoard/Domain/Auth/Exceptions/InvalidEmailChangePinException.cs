using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class InvalidEmailChangePinException(Guid userId) : CustomException(
    ErrorCodes.InvalidPin,
    HttpStatusCode.BadRequest,
    $"Invalid change email verification pin for user with id {userId}"
);