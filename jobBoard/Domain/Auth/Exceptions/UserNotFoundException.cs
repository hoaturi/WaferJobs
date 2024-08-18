using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class UserNotFoundException(Guid userId) : CustomException(
    ErrorCodes.UserNotFound,
    HttpStatusCode.NotFound,
    $"User with id {userId} not found"
);