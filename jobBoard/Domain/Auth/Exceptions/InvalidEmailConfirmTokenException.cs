using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class InvalidEmailConfirmTokenException(Guid userId) : CustomException(
    ErrorCodes.InvalidToken,
    HttpStatusCode.BadRequest,
    $"Invalid email confirmation token for user with id {userId}"
);