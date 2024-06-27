using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class UserNotFoundException(Guid userId) : CustomException(
    ErrorCodes.UserNotFoundError,
    HttpStatusCode.NotFound,
    $"User with id {userId} not found"
);