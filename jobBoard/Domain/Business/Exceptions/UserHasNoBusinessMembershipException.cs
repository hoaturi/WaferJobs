using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class UserHasNoBusinessMembershipException(Guid userId) : CustomException(
    ErrorCodes.UserHasNoBusinessMembership,
    HttpStatusCode.NotFound,
    $"No business membership found for the user {userId}");