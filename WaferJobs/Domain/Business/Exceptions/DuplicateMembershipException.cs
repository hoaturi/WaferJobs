using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class DuplicateMembershipException(Guid userId) : CustomException(
    ErrorCodes.UserAlreadyMember,
    HttpStatusCode.BadRequest,
    $"User {userId} is already a member of a business."
);