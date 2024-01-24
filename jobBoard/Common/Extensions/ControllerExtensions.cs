using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

public static class ResultHandlerExtensions
{
    /// <summary>
    /// Extracts the error status code and return an IActionResult with the error as the body.
    /// </summary>
    [NonAction]
    public static IActionResult HandleFailure(this ControllerBase controller, Error error)
    {
        return new ObjectResult(error) { StatusCode = error.StatusCode, };
    }
}
