using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimantNotFoundException(Guid claimantMemberId) : CustomException(
    ErrorCodes.BusinessClaimantNotFound,
    HttpStatusCode.NotFound,
    $"Business claimant with id: {claimantMemberId} not found."
);