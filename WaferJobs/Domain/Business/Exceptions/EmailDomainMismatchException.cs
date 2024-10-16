using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class EmailDomainMismatchException : CustomException
{
    public EmailDomainMismatchException(Guid userId)
        : base(
            ErrorCodes.EmailDomainMismatch,
            HttpStatusCode.BadRequest,
            $"The email domain of the user {userId} does not match the domain of the business."
        )
    {
    }

    public EmailDomainMismatchException(Guid userId, Guid businessId)
        : base(
            ErrorCodes.EmailDomainMismatch,
            HttpStatusCode.BadRequest,
            $"The email domain of the user {userId} does not match the domain of the business {businessId}"
        )
    {
    }
}