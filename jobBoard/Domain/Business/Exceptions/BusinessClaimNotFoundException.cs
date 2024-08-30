using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessClaimRequestNotFoundException(Guid userId) : CustomException(
    ErrorCodes.BusinessClaimRequestNotFound,
    HttpStatusCode.NotFound,
    $"No valid business claim request found for user {userId}"
);