using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessMembershipNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessMembershipNotFound,
    HttpStatusCode.NotFound,
    $"No members found for business {businessId}");