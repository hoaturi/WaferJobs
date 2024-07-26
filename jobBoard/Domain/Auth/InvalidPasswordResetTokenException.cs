using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class InvalidPasswordResetTokenException(Guid userId) : CustomException(
    ErrorCodes.InvalidToken,
    HttpStatusCode.BadRequest,
    $"Password reset token is invalid for user with id: {userId}"
);