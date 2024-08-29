using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class EmailDomainMismatchException() : CustomException(
    ErrorCodes.EmailDomainMismatch,
    HttpStatusCode.BadRequest,
    "User email domain does not match the business domain."
);