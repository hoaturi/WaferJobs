using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessNotFound,
    HttpStatusCode.NotFound,
    $"Business {businessId} not found."
);