using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class InvalidOrMissingBusinessClaimException(Guid userId)
    : CustomException(ErrorCodes.InvalidOrMissingBusinessClaim, HttpStatusCode.BadRequest,
        $"Business claim not found for user {userId}.");