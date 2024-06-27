using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class BusinessNotFoundForUserException(Guid userId)
    : CustomException(
        ErrorCodes.BusinessNotFoundForUserError,
        HttpStatusCode.NotFound,
        $"User with id: {userId} does not have a business associated with it."
    );