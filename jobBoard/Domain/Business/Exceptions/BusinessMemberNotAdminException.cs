using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class BusinessMemberNotAdminException(Guid businessId, Guid userId) : CustomException(
    ErrorCodes.BusinessMemberNotAdmin,
    HttpStatusCode.Forbidden,
    $"User {userId} is not an admin of business {businessId}"
);