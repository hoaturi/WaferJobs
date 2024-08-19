using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimAlreadyVerifiedException(Guid claimId) : CustomException(
    ErrorCodes.BusinessClaimAlreadyVerified,
    HttpStatusCode.BadRequest,
    $"Business claim with id: {claimId} is already verified."
);