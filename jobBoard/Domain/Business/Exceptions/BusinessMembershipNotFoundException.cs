using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessMembershipNotFoundException(Guid userId) : CustomException(
    ErrorCodes.BusinessMembershipNotFound,
    HttpStatusCode.NotFound,
    $"Business membership not found for user {userId}"
);