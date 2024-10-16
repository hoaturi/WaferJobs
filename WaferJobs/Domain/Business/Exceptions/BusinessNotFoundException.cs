using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class BusinessNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessNotFound,
    HttpStatusCode.NotFound,
    $"Business {businessId} not found."
);