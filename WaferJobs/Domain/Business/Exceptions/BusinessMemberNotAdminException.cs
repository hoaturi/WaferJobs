using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Business.Exceptions;

public class BusinessMemberNotAdminException(Guid businessId, Guid userId) : CustomException(
    ErrorCodes.BusinessMemberNotAdmin,
    HttpStatusCode.Forbidden,
    $"User {userId} is not an admin of business {businessId}"
);