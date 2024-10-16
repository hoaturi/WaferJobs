using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class UserHasNoBusinessMembershipException(Guid userId) : CustomException(
    ErrorCodes.UserHasNoBusinessMembership,
    HttpStatusCode.NotFound,
    $"No business membership found for the user {userId}");