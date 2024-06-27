using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class ChangePasswordFailedException(Guid userId) : CustomException(
    ErrorCodes.InternalServerError,
    HttpStatusCode.InternalServerError,
    $"Failed to change password for user with id: {userId}"
);