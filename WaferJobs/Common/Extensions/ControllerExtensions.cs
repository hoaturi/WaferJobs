using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Models;

namespace WaferJobs.Common.Extensions;

public static class ControllerExtensions
{
    /// <summary>
    ///     Extracts the error status code and return an IActionResult with the error as the body.
    /// </summary>
    [NonAction]
    public static IActionResult HandleError(this ControllerBase controller, Error error)
    {
        var objectResult = new ObjectResult(error)
        {
            StatusCode = error.StatusCode
        };

        return objectResult;
    }
}