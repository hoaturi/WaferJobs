using System.Net;

namespace JobBoard;

public class BusinessLogoUploadFailedException()
    : CustomException(
        ErrorCodes.FileUploadFailed,
        HttpStatusCode.InternalServerError,
        $"Failed to upload business logo"
    );
