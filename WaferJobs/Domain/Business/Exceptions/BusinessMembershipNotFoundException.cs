using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class BusinessMembershipNotFoundException(Guid businessId) : CustomException(
    ErrorCodes.BusinessMembershipNotFound,
    HttpStatusCode.NotFound,
    $"No members found for business {businessId}");