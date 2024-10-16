using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.UpdateMyBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class UpdateMyBusinessController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateBusiness([FromBody] UpdateMyBusinessCommand command)
    {
        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok();
    }
}