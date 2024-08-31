using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class EmailDomainMismatchException : CustomException
{
    public EmailDomainMismatchException(Guid userId)
        : base(
            ErrorCodes.EmailDomainMismatch,
            HttpStatusCode.BadRequest,
            $"The email domain of the user with id {userId} does not match the business website domain."
        )
    {
    }

    public EmailDomainMismatchException(Guid userId, Guid businessId)
        : base(
            ErrorCodes.EmailDomainMismatch,
            HttpStatusCode.BadRequest,
            $"The email domain of the user with id {userId} does not match the domain of the business with id {businessId}."
        )
    {
    }
}