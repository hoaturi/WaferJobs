using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimAlreadyExistsException(Guid businessId) : CustomException(
    ErrorCodes.BusinessClaimAlreadyExists,
    HttpStatusCode.BadRequest,
    $"Business with id {businessId} already has a claim."
);