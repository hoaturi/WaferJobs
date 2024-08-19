using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimInProgressException(Guid businessId) : CustomException(
    ErrorCodes.BusinessClaimInProgress,
    HttpStatusCode.BadRequest,
    $"Business with id: {businessId} has a claim in progress."
);