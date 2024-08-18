using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business;

public class BusinessNotFoundForUserException(Guid userId)
    : CustomException(
        ErrorCodes.BusinessNotFoundForUser,
        HttpStatusCode.NotFound,
        $"User with id: {userId} does not have a business associated with it."
    );