using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class UserAlreadyMemberException(Guid userId) : CustomException(
    ErrorCodes.UserAlreadyMember,
    HttpStatusCode.BadRequest,
    $"User with id: {userId} is already a member of a business."
);