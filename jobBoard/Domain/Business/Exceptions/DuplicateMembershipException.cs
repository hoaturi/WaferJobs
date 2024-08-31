using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class DuplicateMembershipException(Guid userId) : CustomException(
    ErrorCodes.UserAlreadyMember,
    HttpStatusCode.BadRequest,
    $"User {userId} is already a member of the business."
);