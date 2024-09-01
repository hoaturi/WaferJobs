using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessMemberNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessMemberNotFound,
    HttpStatusCode.NotFound,
    $"Business member not found for business {businessId}"
);