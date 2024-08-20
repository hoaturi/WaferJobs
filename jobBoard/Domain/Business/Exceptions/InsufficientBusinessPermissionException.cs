using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class InsufficientBusinessPermissionException(Guid memberId) : CustomException(
    ErrorCodes.InsufficientBusinessPermission,
    HttpStatusCode.Forbidden,
    $"Business member {memberId} does not have sufficient permissions to perform this operation.");