using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Infrastructure.Services.FileUploadService;

public class BusinessLogoUploadFailedException()
    : CustomException(
        ErrorCodes.FileUploadFailedError,
        HttpStatusCode.InternalServerError,
        "Failed to upload business logo"
    );