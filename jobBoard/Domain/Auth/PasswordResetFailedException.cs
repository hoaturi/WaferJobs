using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class PasswordResetFailedException(Guid userId) : CustomException(
    ErrorCodes.InternalServerError,
    HttpStatusCode.InternalServerError,
    $"Failed to reset password for user with id: {userId}"
);