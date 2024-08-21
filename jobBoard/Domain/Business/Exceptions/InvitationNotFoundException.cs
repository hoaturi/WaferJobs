using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Business.Exceptions;

public class InvitationNotFoundException(string token) : CustomException(
    ErrorCodes.InvitationNotFound,
    HttpStatusCode.NotFound,
    "Invitation with token: {token} was not found"
);