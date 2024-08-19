using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessAlreadyClaimedException(Guid businessId) : CustomException(
    ErrorCodes.BusinessAlreadyClaimed,
    HttpStatusCode.BadRequest,
    $"Business with id: {businessId} is already claimed."
);