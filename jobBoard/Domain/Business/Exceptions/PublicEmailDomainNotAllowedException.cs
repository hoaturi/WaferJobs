using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class PublicEmailDomainNotAllowedException(Guid userId) : CustomException(
    ErrorCodes.PublicEmailDomainNotAllowed,
    HttpStatusCode.BadRequest,
    $"The user with id {userId} is using a public email domain.");