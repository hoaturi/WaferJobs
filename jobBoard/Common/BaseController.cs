using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

public abstract class BaseController : ControllerBase
{
    [NonAction]
    public IActionResult HandleFailure(Error error)
    {
        return new ObjectResult(error) { StatusCode = error.StatusCode, };
    }
}
