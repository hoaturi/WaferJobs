using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class UserNotBusinessMemberException(Guid userId) : CustomException(
    ErrorCodes.UserNotBusinessMember,
    HttpStatusCode.Forbidden,
    $"User with id {userId} is not a member of any business"
);