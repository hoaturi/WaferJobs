using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class PublicEmailDomainNotAllowedException(string domain, Guid userId) : CustomException(
    ErrorCodes.PublicEmailDomainNotAllowed,
    HttpStatusCode.BadRequest,
    $"The email domain {domain} of the user {userId} is not allowed."
);