using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business;

public class BusinessNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessNotFound,
    HttpStatusCode.NotFound,
    $"Business profile with id {businessId} not found"
);