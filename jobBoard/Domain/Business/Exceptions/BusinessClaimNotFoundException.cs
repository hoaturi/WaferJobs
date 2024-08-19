using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimNotFoundException(Guid claimId) : CustomException(
    ErrorCodes.BusinessClaimNotFound,
    HttpStatusCode.NotFound,
    $"Business claim with id: {claimId} not found."
);