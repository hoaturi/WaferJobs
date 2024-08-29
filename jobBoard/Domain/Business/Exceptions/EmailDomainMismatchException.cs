using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class EmailDomainMismatchException(Guid userId, Guid businessId) : CustomException(
    ErrorCodes.EmailDomainMismatch,
    HttpStatusCode.BadRequest,
    $"The email domain of the user with id {userId} does not match the domain of the business with id {businessId}."
);