using System.Net;

namespace JobBoard;

public class InvalidBusinessSizeException(int size)
    : CustomException(
        ErrorCodes.InvalidBusinessSize,
        HttpStatusCode.BadRequest,
        $"Invalid business size: {size}. Valid values are: {string.Join(", ", BusinessSize.BusinessSizes.Keys)}."
    );
