using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class PublicEmailDomainNotAllowedException(string domain, Guid userId) : CustomException(
    ErrorCodes.PublicEmailDomainNotAllowed,
    HttpStatusCode.BadRequest,
    $"The email domain {domain} of the user {userId} is not allowed."
);